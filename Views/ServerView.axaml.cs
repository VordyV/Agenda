using System;
using System.Collections.Generic;
using Agenda.Controls;
using Agenda.Core;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Agenda.Views;

public partial class ServerView : UserControl
{
    private Manager _manager;
    private ViewPresenter _presenter;
    private string _connId;
    private BasicDriver _conn;
    private Control _view;
    
    public ServerView()
    {
        InitializeComponent();
    }
    
    public ServerView(Manager manager, ViewPresenter presenter, object? connId)
    {
        this._manager = manager;
        this._presenter = presenter;
        if (!(connId is string)) throw new Exception("");
        InitializeComponent();
        this._connId = (string)connId;
            
        this._conn = this._manager.GetConnection(this._connId);
        var module = this._manager.GetModule(this._conn.ModuleId);
        this._view = module.View.Invoke(this._conn);

        this.MainContent.Content = this._view;
    }
}