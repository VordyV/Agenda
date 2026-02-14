using System;
using System.Collections.Generic;
using Agenda.Core;
using Avalonia.Controls;

namespace Agenda.Controls;

public class ViewPresenter
{
    private Manager _manager;
    private Dictionary<string, Func<Manager, ViewPresenter, object?, UserControl>> _views;
    private string _view;
    public UserControl Content;
    public event Action<Manager, ViewPresenter, object?> OnLoadView;
    
    public ViewPresenter(Manager manager, Dictionary<string, Func<Manager, ViewPresenter, object?, UserControl>>? views = null, string? defaultView = null)
    {
        this._manager = manager;
        if (views != null)
        {
            this._views = views;
            if (defaultView != null) this.LoadView(defaultView);
        }
        else this._views = new();
    }

    public void AddView(string name, Func<Manager, ViewPresenter, object?, UserControl> content)
    {
        if (this._views.ContainsKey(name)) throw new Exception($"View '{name}' already added");
        this._views.Add(name, content);
    }

    public void LoadView(string name, object? arg = null, bool reload = false)
    {
        if (!this._views.ContainsKey(name)) throw new Exception($"View '{name}' does not exist");
        if (this._view == name && !reload) return;
        this._view = name;
        this.Content = this._views[name].Invoke(this._manager, this, arg);
        this.OnLoadView?.Invoke(this._manager, this, arg);
    }
}