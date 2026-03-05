using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

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

public class Manager
{
    private Dictionary<string, Module> _modules = new();
    //private Dictionary<string, BasicDriver> _connections = new();
    private ConcurrentDictionary<string, Connection> _connections = new();

    public event Action<string>? OnCreate;
    public event Action<string, InitContext>? OnInit;
    public event Action<string, DriverState?, bool?>? OnChangeStatus;
    public event Action<string>? OnStop;

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
    
    public string CreateNewConnection(string moduleId, Dictionary<string, object?> fields)
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
        this.OnCreate?.Invoke(connId);
        
        return connId;
    }

    private void SetState(Connection conn, DriverState? state = null, bool? connected = null)
    {
        if (conn.Driver == null) return;
        conn.Driver.SetState(state, connected);
        this.OnChangeStatus?.Invoke(conn.Id, state, connected);
    }

    public async Task<bool> InitConnection(string connId)
    {
        Connection conn = GetConnection(connId);
        InitContext ctx = new InitContext();
        var module = this.GetModule(conn.ModuleId);
        this.OnInit?.Invoke(conn.Id, ctx);
        try
        {
            conn.SetDriver(module.Driver.Invoke(connId));
            //conn.ViewModel.Init();
            conn.Start();
            this.SetState(conn, new DriverState() {Type = TypeDriverState.Starting});
            if (conn.Driver is not null) await conn.Driver.OnStart(ctx, conn.Fields);
            this.SetState(conn, new DriverState() {Type = TypeDriverState.Running}, connected: true);
            ctx.Action("", "", InitCtxAction.Connected);
            _ = this._startConnLoop(conn);
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
        this.OnStop?.Invoke(conn.Id);
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

    private async Task _startConnLoop(Connection conn)
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
        this.OnStop?.Invoke(conn.Id);
        //this._connections.Remove(driver.Id);
    }
}