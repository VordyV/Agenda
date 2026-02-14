using System;
using System.Collections.Generic;
using System.Linq;
using Agenda.Controls;
using Agenda.Core;
using Agenda.Forms;
using Agenda.Forms.ConnectionIndicatorForms;
using Agenda.Views;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Ursa.Controls;

namespace Agenda;

public partial class MainWindow : Window
{
    private Manager _manager;
    private ViewPresenter _viewPresenter;

    public MainWindow()
    {
        InitializeComponent();
    }
    
    public MainWindow(Manager manager)
    {
        this._manager = manager;
        this._viewPresenter = new ViewPresenter(manager: this._manager, 
            views: new()
            {
                {"home", (mgr, ptr, arg) => new HomeView(mgr, ptr)},
                {"server", (mgr, ptr, arg) => new ServerView(mgr, ptr, arg)},
            },
            defaultView: "home"
        );
        
        this._viewPresenter.OnLoadView += this._onLoadView;
        
        InitializeComponent();
        
        this.MainContent.Content = this._viewPresenter.Content;
        
        this._manager.OnInit += this.OnCreateInit;
    }

    private void _onLoadView(Manager manager, ViewPresenter presenter, object? arg)
    {
        this.MainContent.Content = presenter.Content;
    }

    private void MenuItemOpen_OnClick(object? sender, RoutedEventArgs e) => this._viewPresenter.LoadView("home");

    private void MenuItemConnect_OnClick(object? sender, RoutedEventArgs e)
    {
        var context = new DialogContext();
        OverlayDialog.ShowCustom(new ConnectForm(this._manager, this._viewPresenter) {DataContext = context}, context, hostId: "main", new OverlayDialogOptions() {CanDragMove = false, CanResize = false});
    }
    
    public async void OnCreateInit(string connId, InitContext ctx)
    {
        var conn = this._manager.GetConnection(connId);
        var ctxPcsForm = new DialogContext();
        var form = new ProcessIndicatorForm() {DataContext = ctxPcsForm};

        ctx.OnAction += async (s, t, c) =>
        {
            form.SetStatus(s);
            form.SetText(t);
            if (c != null) ctxPcsForm.Close();
            if (c == true)
            {
                this._viewPresenter.LoadView("server", connId);
            } else if (c == false)
            {
                var ctxErrForm = new DialogContext();
                OverlayDialog.ShowCustom(new ErrorIndicatorForm(s, t, async (o, args) => ctxErrForm.Close()) {DataContext = ctxErrForm}, ctxErrForm, hostId: "main");
            }
        };
        
        await OverlayDialog.ShowCustomModal<bool>(form, ctxPcsForm, hostId: "main", new OverlayDialogOptions() {IsCloseButtonVisible = false});
    }
}