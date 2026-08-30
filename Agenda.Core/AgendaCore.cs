using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BLite.Bson;
using BLite.Core;
using BLite.Core.Collections;
using BLite.Core.Query;
using ZeroAlloc.AsyncEvents;

namespace Agenda.Core;

public enum InitCtxAction
{
    Connected,
    Cancelled,
    Error
}

public class InitContext
{
    public event Action<string, string, InitCtxAction?>? OnAction;
    public void Action(string status, string text, InitCtxAction? action = null) => this.OnAction?.Invoke(status, text, action);
}

//public class ProfileDuplicationException : Exception { public ProfileDuplicationException(string message) : base(message) { } }

public class AgendaCore
{
    private Dictionary<string, Module> _modules = new();
    //private Dictionary<string, BasicDriver> _connections = new();
    private ConcurrentDictionary<string, Connection> _connections = new();
    private ConcurrentDictionary<string, Connection> _previewConnections = new();
    private AppDbContext _appDbContext;

    // OnCreateConn
    private AsyncEventHandler<CreateConnEventArgs> _onCreateConn = new(InvokeMode.Parallel);
    public event AsyncEvent<CreateConnEventArgs> OnCreateConn { add => _onCreateConn.Register(value); remove => _onCreateConn.Unregister(value); }
    
    // OnInitConn
    private AsyncEventHandler<InitConnEventArgs> _onInitConn = new(InvokeMode.Parallel);
    public event AsyncEvent<InitConnEventArgs> OnInitConn { add => _onInitConn.Register(value); remove => _onInitConn.Unregister(value); }
    
    // OnChangeStatusConn
    private AsyncEventHandler<ChangeStatusConnEventArgs> _onChangeStatusConn = new(InvokeMode.Parallel);
    public event AsyncEvent<ChangeStatusConnEventArgs> OnChangeStatusConn { add => _onChangeStatusConn.Register(value); remove => _onChangeStatusConn.Unregister(value); }
    
    // OnStopConn
    private AsyncEventHandler<StopConnConnEventArgs> _onStopConn = new(InvokeMode.Parallel);
    public event AsyncEvent<StopConnConnEventArgs> OnStopConn { add => _onStopConn.Register(value); remove => _onStopConn.Unregister(value); }
    
    // OnError
    private AsyncEventHandler<ErrorEventArgs> _onError = new(InvokeMode.Parallel);
    public event AsyncEvent<ErrorEventArgs> OnError { add => _onError.Register(value); remove => _onError.Unregister(value); }
    
    // OnReady
    private AsyncEventHandler<bool> _onReady = new(InvokeMode.Parallel);
    public event AsyncEvent<bool> OnReady { add => _onReady.Register(value); remove => _onReady.Unregister(value); }

    public bool IsReady { get; private set; } = false;

    public AgendaCore()
    {
        
    }

    public async Task Init()
    {
        try
        {
            await Task.Run(() =>
            {
                string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "agenda");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                string path = Path.Combine(dir, "agenda.db");
                this._appDbContext = new AppDbContext(path);
            });

            this.IsReady = true;
            this._onReady.InvokeAsync(IsReady);
        }
        catch (Exception e)
        {
            this._error(e);
        }
    }
    
    private void _error(Exception error) => this._onError.InvokeAsync(new ErrorEventArgs() {Error = error});
    
    public void RegisterModule(Module module)
    {
        if (this._modules.ContainsKey(module.Id)) throw new Exception($"Module {module.Id} is already registered");
        this._modules.Add(module.Id, module);
    }

    public void RegisterModules(List<Module> modules)
    {
        foreach (var m in modules)
        {
            this.RegisterModule(module: m);
        }
    }

    public List<Module> GetModules() => this._modules.Values.ToList();

    public Module GetModule(string moduleId)
    {
        if (!this._modules.ContainsKey(moduleId)) throw new Exception($"Module with id {moduleId} not found");
        return this._modules[moduleId];
    }

    public Connection GetConnection(string connId)
    {
        if (!this._connections.ContainsKey(connId)) throw new Exception($"Connection with id {connId} not found");
        return this._connections[connId];
    }

    public List<Connection> GetConnections() => this._connections.Values.ToList();

    public List<Connection> GetActiveConnections()
    {
        List<Connection> result = new();
        foreach (var conn in this._connections.Values)
        {
            if (conn.Driver != null && conn.Driver.Connected) result.Add(conn);
        }
        return result;
    }
    
    public string CreateNewConnection(string moduleId, Dictionary<string, string?> fields)
    {
        Module module = this.GetModule(moduleId);
        Guid uuid = Guid.NewGuid();
        string connId = uuid.ToString("N");
        
        var conn = new Connection(id: connId, moduleId: moduleId, fields: fields);
        this._connections.TryAdd(connId, conn);
        //this._connections.Add(connId, conn);
        var view = module.View.Invoke(conn);
        conn.SetView(view);
        //this._connections.Add(connId, module.Driver.Invoke(connId, moduleId, fields));
        this._onCreateConn.InvokeAsync(new CreateConnEventArgs() {ConnectionId = connId});
        
        return connId;
    }

    private void SetState(Connection conn, DriverState? state = null, bool? connected = null)
    {
        if (conn.Driver == null) return;
        conn.Driver.SetState(state, connected);
        this._onChangeStatusConn.InvokeAsync(new ChangeStatusConnEventArgs() {ConnectionId = conn.Id, State = state, IsConnected = connected});
    }

    public async Task<bool> InitConnection(string connId, bool silentSession = false)
    {
        Connection conn = GetConnection(connId);
        InitContext ctx = new InitContext();
        var module = this.GetModule(conn.ModuleId);
        if (!silentSession) this._onInitConn.InvokeAsync(new InitConnEventArgs() {ConnectionId = conn.Id, Context = ctx});
        try
        {
            conn.SetDriver(module.GetDriverInstance(connId));
            //conn.ViewModel.Init();
            conn.Start();
            this.SetState(conn, new DriverState() {Type = TypeDriverState.Starting});
            if (conn.Driver is not null) await conn.Driver.OnStart(ctx, conn.Fields);
            this.SetState(conn, new DriverState() {Type = TypeDriverState.Running}, connected: true);
            ctx.Action("", "", InitCtxAction.Connected);
            _ = this._startConnLoop(conn, silentSession);
            return true;
        }
        catch (OperationCanceledException e)
        {
            this.SetState(conn, new DriverState() {Type = TypeDriverState.Cancelled, ErrorDetail = e.Message});
            ctx.Action("", e.Message, InitCtxAction.Cancelled);
        }
        catch (InitException e)
        {
            this.SetState(conn, new DriverState() {Type = TypeDriverState.Error, ErrorDetail = e.Message});
            ctx.Action(e.Title, e.Message, InitCtxAction.Error);
        }
        catch (Exception e)
        {
            this.SetState(conn, new DriverState() {Type = TypeDriverState.Error, ErrorDetail = e.Message});
            ctx.Action("Error 0_o", e.Message, InitCtxAction.Error);
            //throw;
        }
        
        //conn.ViewModel.Detach();
        conn.Stop();
        conn.DisposeDriver();
        if (!silentSession) this._onStopConn.InvokeAsync(new StopConnConnEventArgs() {ConnectionId = conn.Id});
        //this._connections.Remove(connId);
        return false;
        //Task task = Task.Run(() => this._startConnLoop(driver));
    }

    public void CloseConnection(string connId)
    {
        Connection conn = GetConnection(connId);
        conn.Driver?.Cancel();
    }

    public void RemoveConnection(string connId)
    {
        Connection conn = GetConnection(connId);
        if (conn.Driver != null) throw new Exception("Connection is not closed");
        this._connections.TryRemove(connId, out Connection? value);
    }

    private async Task _startConnLoop(Connection conn, bool silentSession)
    {
        try
        {
            if (conn.Driver is not null) await conn.Driver.OnLoop();
            this.SetState(conn, new DriverState() { Type = TypeDriverState.Stopped }, connected: false);
        }
        catch (OperationCanceledException e)
        {
            this.SetState(conn, new DriverState() {Type = TypeDriverState.Cancelled, ErrorDetail = e.Message});
        }
        catch (Exception e)
        {
            this.SetState(conn, new DriverState() { Type = TypeDriverState.Error, ErrorDetail = e.Message });
        }
        
        try
        {
            if (conn.Driver is not null) await conn.Driver.OnStop();
        }
        catch (Exception e)
        {
            Debug.WriteLine($"ERR {e.Message}");
        }
        
        //conn.ViewModel.Detach();
        this.SetState(conn, connected: false);
        conn.Stop();
        conn.DisposeDriver();
        if (!silentSession) this._onStopConn.InvokeAsync(new StopConnConnEventArgs() {ConnectionId = conn.Id});
        //this._connections.Remove(driver.Id);
    }

    public async Task AddProfile(string name, string moduleId, Dictionary<string, string?> fields)
    {
        var profileModel = this._appDbContext.Profiles;
        if (await profileModel.AsQueryable().Where(x => x.Name == name).CountAsync() > 0) throw new Exception($"Cannot add a profile with that name '{name}' because the name is already used by another profile");
        // TODO: duplicate field check doesn't work
        //if (await profileModel.AsQueryable().Where(x => Enumerable.SequenceEqual(x.Fields, fields)).CountAsync() > 0) throw new Exception($"A profile with exactly the same field values already exists.");
        this.GetModule(moduleId);
        
        ProfileModel profile = new ProfileModel() {Name = name, ModuleId = moduleId, Fields = fields, LastSessionDate = DateTime.Now};

        await profileModel.InsertAsync(profile);
    }

    public IAsyncEnumerable<ProfileModel> GetAllProfiles()
    {
        var profileModel = this._appDbContext.Profiles;
        return profileModel.FindAllAsync();
    }

    public async Task DeleteProfile(ObjectId objectId)
    {
        var profileModel = this._appDbContext.Profiles;
        await profileModel.DeleteAsync(objectId);
    }
    
    public async Task<ProfileModel?> GetProfile(ObjectId objectId)
    {
        var profileModel = this._appDbContext.Profiles;
        return await profileModel.FindByIdAsync(objectId);
    }

    public async Task UpdateProfile(ProfileModel profile)
    {
        var profileModel = this._appDbContext.Profiles;
        await profileModel.UpdateAsync(profile);
    }

    public async Task Dispose()
    {
        await this._appDbContext.DisposeAsync();
    }

    public async Task<Preview> RequestPreviewData(ObjectId objectId, TimeSpan timeout)
    {
        ProfileModel? profileModel = await this.GetProfile(objectId);
        if (profileModel == null) throw new Exception($"No profile with ID {objectId}");
        
        Preview preview;
        
        Connection conn = new Connection(id: objectId.ToString(), profileModel.ModuleId, profileModel.Fields);
        this._previewConnections.TryAdd(objectId.ToString(), conn);
        Module module = this.GetModule(profileModel.ModuleId);
        BasicDriver driver = module.GetDriverInstance(conn.Id);
        conn.SetDriver(driver);
        
        try
        {
            await driver.OnStart(new InitContext(), profileModel.Fields).WaitAsync(timeout);
            preview = await driver.OnPreview().WaitAsync(timeout);
        }
        catch (Exception e)
        {
            preview = await driver.OnPreviewError();
        }

        driver.Cancel();
        conn.DisposeDriver();
        this._previewConnections.TryRemove(objectId.ToString(), out _);

        return preview;
    }

    public async Task CancelRequestPreviewData(ObjectId objectId, TimeSpan timeout)
    {
        Connection? connection;
        if (!this._previewConnections.TryGetValue(objectId.ToString(), out connection)) return;
        connection.Driver?.Cancel();
        await Task.Run(() =>
        {
            while (this._previewConnections.ContainsKey(objectId.ToString()))
            {
                
            }
        }).WaitAsync(timeout);
    }

    public bool IsActiveRequestPreviewData(ObjectId objectId) => this._previewConnections.ContainsKey(objectId.ToString());
}