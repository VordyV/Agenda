using System.Collections.Generic;
using Agenda.Controls;
using Agenda.Core;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Agenda.Views;

public partial class ServerView : UserControl
{
    private Manager _manager;
    private ViewPresenter _presenter;
    private string _connId;
    
    public ServerView(Manager manager, ViewPresenter presenter, object? connId)
    {
        this._manager = manager;
        this._presenter = presenter;
        if (connId is string) this._connId = (string)connId;
        InitializeComponent();
        this.T.Text = this._connId;
    }
    
}