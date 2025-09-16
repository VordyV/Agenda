using System;
using System.Collections.Generic;
using System.Data;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Agenda.App;

public class PresenterView : ObservableObject
{
    public ContentControl View;
    
    private Manager _manager;
    private Dictionary<string, Func<Manager, PresenterView, object?, UserControl>> _views = new Dictionary<string, Func<Manager, PresenterView, object?, UserControl>>();

    public PresenterView(Manager manager)
    {
        View = new ContentControl();
        _manager = manager;
    }
    
    public void Add(string name, Func<Manager, PresenterView, object?, UserControl> view)
    {
        if (_views.ContainsKey(name)) throw new DuplicateNameException($"View {name} has already been added");
        _views.Add(name, view);
    }

    public void LoadNew(string name, object? arg = null)
    {
        if (!_views.ContainsKey(name)) throw new Exception($"View {name} was not found");
        View.Content = _views[name](_manager, this, arg);
    }
}