using System;
using System.Collections.Generic;
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
    public event Action<string, string, InitCtxAction?> OnAction;
    public void Action(string status, string text, InitCtxAction? action = null) => this.OnAction?.Invoke(status, text, action);
}

public class Manager
{
    private Dictionary<string, Module> _modules = new();
    private Dictionary<string, BasicDriver> _connections = new();
    
    public event Action<string> OnCreate;
    public event Action<string, InitContext> OnInit;
    public event Action<string, DriverState?, bool?> OnChangeStatus;
    public event Action<string> OnStop; 

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

    public BasicDriver GetConnection(string connId)
    {
        if (!this._connections.ContainsKey(connId)) throw new Exception($"Connection with id {connId} not found");
        return this._connections[connId];
    }

    public List<BasicDriver> GetConnections() => this._connections.Values.ToList();

    public List<BasicDriver> GetActiveConnections()
    {
        List<BasicDriver> result = new();
        foreach (var conn in this._connections.Values)
        {
            if (conn.Connected) result.Add(conn);
        }
        return result;
    }
    
    public string CreateNewConnection(string moduleId, Dictionary<string, object?> fields)
    {
        Module module = this.GetModule(moduleId);
        Guid uuid = Guid.NewGuid();
        string connId = uuid.ToString("N");

        this._connections.Add(connId, module.Driver.Invoke(connId, moduleId, fields));
        this.OnCreate?.Invoke(connId);
        
        return connId;
    }

    private void SetState(BasicDriver driver, DriverState? state = null, bool? connected = null)
    {
        driver.SetState(state, connected);
        this.OnChangeStatus?.Invoke(driver.Id, state, connected);
    }

    public async Task<bool> InitConnection(string connId)
    {
        BasicDriver driver = GetConnection(connId);
        InitContext ctx = new InitContext();
        this.OnInit?.Invoke(driver.Id, ctx);
        try
        {
            this.SetState(driver, new DriverState() {Type = TypeDriverState.Starting});
            await driver.OnStart(ctx);
            this.SetState(driver, new DriverState() {Type = TypeDriverState.Running}, connected: true);
            ctx.Action("", "", InitCtxAction.Connected);
            Task task = Task.Run(() => this._startConnLoop(driver));
            return true;
        }
        catch (OperationCanceledException e)
        {
            this.SetState(driver, new DriverState() {Type = TypeDriverState.Error, ErrorDetail = e.Message});
            ctx.Action("", e.Message, InitCtxAction.Cancelled);
        }
        catch (InitException e)
        {
            this.SetState(driver, new DriverState() {Type = TypeDriverState.Error, ErrorDetail = e.Message});
            ctx.Action(e.Title, e.Message, InitCtxAction.Error);
        }
        catch (Exception e)
        {
            this.SetState(driver, new DriverState() {Type = TypeDriverState.Error, ErrorDetail = e.Message});
            ctx.Action("Error 0_o", e.Message, InitCtxAction.Error);
            //throw;
        }
        this.OnStop?.Invoke(connId);
        this._connections.Remove(connId);
        return false;
        //Task task = Task.Run(() => this._startConnLoop(driver));
    }

    public async Task CloseConnection(string connId)
    {
        BasicDriver driver = GetConnection(connId);
        driver.Cancel();
    }

    private async void _startConnLoop(BasicDriver driver)
    {
        try
        {
            await driver.OnLoop();
            this.SetState(driver, new DriverState() { Type = TypeDriverState.Stopped }, connected: false);
        }
        catch (Exception e)
        {
            this.SetState(driver, new DriverState() { Type = TypeDriverState.Error, ErrorDetail = e.Message });
        }
        finally
        {
            await driver.OnStop();
            this.SetState(driver, connected: false);
            this.OnStop?.Invoke(driver.Id);
            driver.Dispose();
        }
        this._connections.Remove(driver.Id);
    }
}