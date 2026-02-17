using System;
using System.Collections.Generic;
using Agenda.Controls;
using Agenda.Core;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Agenda.Views;

public partial class ServerView : UserControl
{
    private Manager _manager;
    private ViewPresenter _presenter;
    private string _connId;
    private Connection _conn;
    
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

        this.MainContent.Content = this._conn.View;
    }
}