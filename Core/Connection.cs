using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Agenda.Core;

public class Connection
{
    public string Id { get; private set; }
    public string ModuleId { get; private set; }
    public UserControl View { get; private set; }
    public BasicDriver? Driver { get; private set; }
    public Dictionary<string, object?> Fields { get; private set; }
    public event Action OnStart; 
    public event Action OnStop;
    public bool IsStarted { get; private set; } = false;

    public Connection(string id, string moduleId, Dictionary<string, object?> fields)
    {
        this.Id = id;
        this.ModuleId = moduleId;
        this.Fields = fields;
        Debug.WriteLine($"[{this.Id}] Connection created");
        this.OnStart += () => this.IsStarted = true;
        this.OnStop += () => this.IsStarted = false;
    }
    
    ~Connection() {
        Debug.WriteLine($"[{this.Id}] Connection deleted");
    }

    public void SetView(UserControl view)
    {
        this.View = view;
    }

    public void SetDriver(BasicDriver driver)
    {
        if (this.Driver != null) throw new Exception("");
        this.Driver = driver;
    }

    public void DisposeDriver()
    {
        if (this.Driver == null) throw new Exception("");
        this.Driver.Dispose();
        this.Driver = null;
    }

    public void Start() => this.OnStart?.Invoke();
    public void Stop() => this.OnStop?.Invoke();
}