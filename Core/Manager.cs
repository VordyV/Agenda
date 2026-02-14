using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agenda.Core;

public class Manager
{
    private Dictionary<string, Module> _modules = new();
    private Dictionary<string, BasicDriver> _connections = new();
    
    public event Action<string> OnCreate;
    public event Action<string, BasicDriver> OnInit;
    public event Action<string, DriverState?, bool?> OnChangeStatus;

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
    
    public string CreateNewConnection(string moduleId, Dictionary<string, object?> fields)
    {
        Module module = this.GetModule(moduleId);
        Guid uuid = Guid.NewGuid();
        string connId = uuid.ToString("N");

        this._connections.Add(connId, module.Driver.Invoke(connId, fields));
        this.OnCreate?.Invoke(connId);
        
        return connId;
    }

    private void SetState(BasicDriver driver, DriverState? state = null, bool? connected = null)
    {
        driver.SetState(state, connected);
        this.OnChangeStatus?.Invoke(driver.Id, state, connected);
    }

    public async Task InitConnection(string connId)
    {
        BasicDriver driver = GetConnection(connId);
        this.OnInit?.Invoke(connId, driver);
        try
        {
            this.SetState(driver, new DriverState() {Type = TypeDriverState.Starting});
            await driver.OnStart();
            this.SetState(driver, new DriverState() { Type = TypeDriverState.Running }, connected: true);
        }
        catch (Exception e)
        {
            this.SetState(driver, new DriverState() {Type = TypeDriverState.Error, ErrorDetail = e.Message});
            throw;
        }

        Task task = Task.Run(() => this._startConnLoop(driver));
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
            driver.Dispose();
        }

        this._connections.Remove(driver.Id);
    }
}