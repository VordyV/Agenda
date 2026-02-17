using System;
using System.Collections.Generic;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Agenda.Core;

public class Connection
{
    public string Id { get; private set; }
    public string ModuleId { get; private set; }
    public UserControl View { get; private set; }
    public BasicViewModel ViewModel { get; private set; }
    public BasicDriver? Driver { get; private set; }
    public Dictionary<string, object?> Fields { get; private set; }

    public Connection(string id, string moduleId, Dictionary<string, object?> fields)
    {
        this.Id = id;
        this.ModuleId = moduleId;
        this.Fields = fields;
    }

    public void SetView(UserControl view, BasicViewModel viewModel)
    {
        this.View = view;
        this.ViewModel = viewModel;
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
}