using System;
using System.Collections.Generic;
using Agenda.Core;
using Avalonia.Controls;

namespace Agenda.Controls;

public class ViewPresenter
{
    private Manager _manager;
    private Dictionary<string, Func<Manager, ViewPresenter, object?, UserControl>> _views;
    private string? _defaultView;
    private string _view;
    private Dictionary<string, UserControl> _viewInstances;
    public UserControl Content;
    public event Action<Manager, ViewPresenter, object?> OnLoadView;
    
    public ViewPresenter(Manager manager, Dictionary<string, Func<Manager, ViewPresenter, object?, UserControl>>? views = null, string? defaultView = null)
    {
        this._manager = manager;
        this._viewInstances = new();
        this._defaultView = defaultView;
        if (views != null)
        {
            this._views = views;
            if (this._defaultView != null) this.LoadView(this._defaultView);
        }
        else this._views = new();
    }

    public void AddView(string name, Func<Manager, ViewPresenter, object?, UserControl> content)
    {
        if (this._views.ContainsKey(name)) throw new Exception($"View '{name}' already added");
        this._views.Add(name, content);
    }

    // Creates a new view instance every time
    public void LoadView(string name, object? arg = null, bool reload = false)
    {
        if (!this._views.ContainsKey(name)) throw new Exception($"View '{name}' does not exist");
        if (this._view == name && !reload) return;
        this._view = name;
        
        if (this._viewInstances.ContainsKey(name)) this._viewInstances.Remove(name);
        this._viewInstances.Add(name, this._views[name].Invoke(this._manager, this, arg));
        
        this.Content = this._viewInstances[name];
        this.OnLoadView?.Invoke(this._manager, this, arg);
    }

    // Creates a view instance only once, if it hasn't been created yet
    public void ShowView(string name, object? arg = null)
    {
        if (!this._views.ContainsKey(name)) throw new Exception($"View '{name}' does not exist");
        if (this._view == name) return;
        this._view = name;
        
        if (!this._viewInstances.ContainsKey(name)) this._viewInstances.Add(name, this._views[name].Invoke(this._manager, this, arg));
        
        this.Content = this._viewInstances[name];
        this.OnLoadView?.Invoke(this._manager, this, arg);
    }

    public void CloseView(string name)
    {
        if (this._viewInstances.ContainsKey(name)) this._viewInstances.Remove(name);
        if (this._view == name && this._defaultView != null) this.LoadView(this._defaultView);
    }
}