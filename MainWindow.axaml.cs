using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Agenda.Controls;
using Agenda.Core;
using Agenda.Forms;
using Agenda.Forms.ConnectionIndicatorForms;
using Agenda.Views;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Ursa.Controls;
using WindowNotificationManager = Ursa.Controls.WindowNotificationManager;
using Notification = Ursa.Controls.Notification;

namespace Agenda;

public partial class MainWindow : Window
{
    private Manager _manager;
    private ViewPresenter _viewPresenter;
    private WindowNotificationManager _notificationManager;

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
        this._notificationManager = new WindowNotificationManager(this);
        this._notificationManager.Position = NotificationPosition.TopRight;
        
        this._viewPresenter.OnLoadView += this._onLoadView;
        
        InitializeComponent();

        this.Title = $"Agenda";
        
        this.MainContent.Content = this._viewPresenter.Content;

        this._manager.OnCreate += this.OnCreate;
        this._manager.OnInit += this.OnInit;
        this._manager.OnStop += this.OnStop;
        this._manager.OnChangeStatus += this.OnChangeStatus;
    }

    private void OnChangeStatus(string connId, DriverState? state, bool? connected)
    {
        Debug.WriteLine($"[{connId}] state={state?.Type.ToString()} connected={connected?.ToString()}");
        if (state is not null && state.Type == TypeDriverState.Error)
        {
            Debug.WriteLine($"[{connId}] {state.ErrorDetail}");
            this._notificationManager.Show(
                new Notification("Session ended unexpectedly", state.ErrorDetail),
                showIcon: true,
                showClose: true,
                type: NotificationType.Error);
        }
    }

    private void _onLoadView(string view)
    {
        this.MainContent.Content = this._viewPresenter.Content;
    }

    private void MenuItemOpen_OnClick(object? sender, RoutedEventArgs e)
    {
        this._viewPresenter.LoadView("home");
        this._viewPresenter.CloseView("server");
    } 

    private void MenuItemConnect_OnClick(object? sender, RoutedEventArgs e)
    {
        var context = new DialogContext();
        OverlayDialog.ShowCustom(new ConnectForm(this._manager) {DataContext = context}, context, hostId: "main", new OverlayDialogOptions() {CanDragMove = false, CanResize = false});
    }

    public async void OnCreate(string connId)
    {
        foreach (var conn in this._manager.GetConnections())
        {
            if (conn.Id == connId) continue;
            this._manager.RemoveConnection(conn.Id);
        }
    }
    
    public async void OnInit(string connId, InitContext ctx)
    {
        var conn = this._manager.GetConnection(connId);
        var ctxPcsForm = new DialogContext();
        var form = new ProcessIndicatorForm((o, args) => conn.Driver?.Cancel()) {DataContext = ctxPcsForm};

        ctx.OnAction += async (s, t, c) =>
        {
            form.SetStatus(s);
            form.SetText(t);
            if (c != null || c == InitCtxAction.Cancelled) ctxPcsForm.Close();
            if (c == InitCtxAction.Connected)
            {
                this._viewPresenter.LoadView("server", connId, reload: true);
                this.MenuItemGoToActive.IsEnabled = true;
                this.MenuItemCloseActive.IsEnabled = true;
            } else if (c == InitCtxAction.Error)
            {
                var ctxErrForm = new DialogContext();
                OverlayDialog.ShowCustom(new ErrorIndicatorForm(s, t, async (o, args) => ctxErrForm.Close()) {DataContext = ctxErrForm}, ctxErrForm, hostId: "main");
            }
        };
        
        await OverlayDialog.ShowCustomModal<bool>(form, ctxPcsForm, hostId: "main", new OverlayDialogOptions() {IsCloseButtonVisible = false});
    }

    public void OnStop(string connId)
    {
        this._manager.RemoveConnection(connId);
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            this.MenuItemGoToActive.IsEnabled = false;
            this.MenuItemCloseActive.IsEnabled = false;
        });
        
    }

    private void MenuItemGoToActive_OnClick(object? sender, RoutedEventArgs e) => this._viewPresenter.ShowView("server");

    private void MenuItemCloseActive_OnClick(object? sender, RoutedEventArgs e)
    {
        // Since only one session can be open at a time for now, there will be only one element among the active ones
        var conns = this._manager.GetActiveConnections();
        if (conns.Count < 1) return;
        var conn = conns[0];
        conn.Driver?.Cancel();
    }

    private void MenuItemAbout_OnClick(object? sender, RoutedEventArgs e)
    {
        var context = new DialogContext();
        OverlayDialog.ShowCustom(new AboutForm() {DataContext = context}, context, hostId: "main", new OverlayDialogOptions() {CanDragMove = true, CanResize = false});
    }

    private void MenuItemWebsite_OnClick(object? sender, RoutedEventArgs e)
    {
        TopLevel.GetTopLevel(this)?.Launcher.LaunchUriAsync(new Uri(Settings.GithubUrl));
    }

    private void MenuItemReportBug_OnClick(object? sender, RoutedEventArgs e)
    {
        TopLevel.GetTopLevel(this)?.Launcher.LaunchUriAsync(new Uri(Settings.BugReportUrl));
    }
}