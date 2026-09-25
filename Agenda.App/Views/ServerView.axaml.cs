using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Agenda.Controls;
using Agenda.Core;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;
using Ursa.Controls;

namespace Agenda.Views;

public partial class ServerView : UserControl
{
    private AgendaCore _agendaCore;
    private ViewPresenter _presenter;
    private string _connId;
    private Connection _conn;
    
    public ServerView()
    {
        InitializeComponent();
    }
    
    public ServerView(AgendaCore agendaCore, ViewPresenter presenter, object? connId)
    {
        this._agendaCore = agendaCore;
        this._presenter = presenter;
        if (!(connId is string)) throw new Exception("");
        InitializeComponent();
        this._connId = (string)connId;
        this._conn = this._agendaCore.GetConnection(this._connId);
        
        this.MainContent.Content = this._conn.View;

        this._agendaCore.OnChangeStatusConn += this.OnChangeStatusConn;
        this.SetStatus(this._conn.Driver.State);

        Module module = this._agendaCore.GetModule(this._conn.ModuleId);
        this.LabelFields.Content = new MarkdownLine() {Markdown = module.GetSubtitle(this._conn.Fields)};
    }

    private async ValueTask OnChangeStatusConn(ChangeStatusConnEventArgs eventArgs, CancellationToken token)
    {
        if (eventArgs.ConnectionId != this._connId) return;
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (eventArgs.State is not null) this.SetStatus(eventArgs.State);
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