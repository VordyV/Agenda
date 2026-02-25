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

        this._manager.OnChangeStatus += this.OnChangeStatus;
        this.SetStatus(this._conn.Driver.State);
    }

    private void OnChangeStatus(string connId, DriverState? state, bool? connected)
    {
        if (connId != this._connId) return;
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (state is not null) this.SetStatus(state);
        });
    }

    private void SetStatus(DriverState state)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            this.LabelStatus.Content = state.Type;
        });
    }
}