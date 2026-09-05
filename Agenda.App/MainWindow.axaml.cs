using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
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
using Velopack;
using WindowNotificationManager = Ursa.Controls.WindowNotificationManager;
using Notification = Ursa.Controls.Notification;

namespace Agenda;

public partial class MainWindow : Window
{
    private AgendaCore _agendaCore;
    private ViewPresenter _viewPresenter;
    private WindowNotificationManager _notificationManager;

    public MainWindow()
    {
        InitializeComponent();
    }
    
    public MainWindow(AgendaCore agendaCore)
    {
        this._agendaCore = agendaCore;
        this._viewPresenter = new ViewPresenter(manager: this._agendaCore, 
            views: new()
            {
                {"home", (mgr, ptr, arg) => new HomeView(mgr, ptr)},
                {"server", (mgr, ptr, arg) => new ServerView(mgr, ptr, arg)},
            },
            defaultView: "home"
        );
        this._notificationManager = new WindowNotificationManager(this);
        this._notificationManager.Position = NotificationPosition.TopRight;
        
        NotificationManager.Init(this._notificationManager);
        
        this._viewPresenter.OnLoadView += this._onLoadView;
        
        InitializeComponent();

        this.Title = $"Agenda";
        
        this.MainContent.Content = this._viewPresenter.Content;
        
        this._agendaCore.OnCreateConn += this.OnCreateConn;
        this._agendaCore.OnInitConn += this.OnInitConn;
        this._agendaCore.OnStopConn += this.OnStopConn;
        this._agendaCore.OnChangeStatusConn += this.OnChangeStatusConn;
        this._agendaCore.OnError += this.OnError;
        
        this.Loaded += async (sender, args) => await this._onLoaded(sender, args);
    }

    private async Task _onLoaded(object? sender, RoutedEventArgs args)
    {
        Updater updater = new Updater(Settings.UpdaterGitHubRep, Settings.UpdaterPrerelease);
        try
        {
            UpdateInfo? updateInfo = await updater.CheckUpdate();
            //if (updateInfo != null) await updater.Update(updateInfo);
        }
        catch (Velopack.Exceptions.NotInstalledException)
        {

        }
        catch (Exception e)
        {
            Debug.WriteLine($"Failed to check for and install the update: {e.Message}");
        }
        await this._agendaCore.Init();
    }

    private async ValueTask OnError(ErrorEventArgs eventArgs, CancellationToken token)
    {
        Dialog.ShowStandard(new SelectableTextBlock() {Text = eventArgs.Error.Message}, null, this, new DialogOptions() {Title = "An unexpected error occurred", Mode = DialogMode.Error, Button = DialogButton.OK});
    }
    
    private async ValueTask OnChangeStatusConn(ChangeStatusConnEventArgs eventArgs, CancellationToken token)
    {
        Debug.WriteLine($"[{eventArgs.ConnectionId}] state={eventArgs.State?.Type.ToString()} connected={eventArgs.IsConnected?.ToString()}");
        if (eventArgs.State is not null && eventArgs.State.Type == TypeDriverState.Error)
        {
            Debug.WriteLine($"[{eventArgs.ConnectionId}] {eventArgs.State?.ErrorDetail}");
            this._notificationManager.Show(
                new Notification("Session ended unexpectedly", eventArgs.State?.ErrorDetail),
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
        OverlayDialog.ShowCustom(new ConnectForm(this._agendaCore) {DataContext = context}, context, hostId: "main", new OverlayDialogOptions() {CanDragMove = false, CanResize = false});
    }

    public async ValueTask OnCreateConn(CreateConnEventArgs eventArgs, CancellationToken token)
    {
        foreach (var conn in this._agendaCore.GetConnections())
        {
            if (conn.Id == eventArgs.ConnectionId) continue;
            this._agendaCore.RemoveConnection(conn.Id);
        }
    }
    
    public async ValueTask OnInitConn(InitConnEventArgs eventArgs, CancellationToken token)
    {
        var conn = this._agendaCore.GetConnection(eventArgs.ConnectionId);
        var ctxPcsForm = new DialogContext();
        var form = new ProcessIndicatorForm((o, args) => conn.Driver?.Cancel()) {DataContext = ctxPcsForm};

        eventArgs.Context.OnAction += async (s, t, c) =>
        {
            form.SetStatus(s);
            form.SetText(t);
            if (c != null || c == InitCtxAction.Cancelled) ctxPcsForm.Close();
            if (c == InitCtxAction.Connected)
            {
                this._viewPresenter.LoadView("server", eventArgs.ConnectionId, reload: true);
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

    public async ValueTask OnStopConn(StopConnConnEventArgs eventArgs, CancellationToken token)
    {
        try
        {
            this._agendaCore.RemoveConnection(eventArgs.ConnectionId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
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
        var conns = this._agendaCore.GetActiveConnections();
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

    private async void MenuItemNew_OnClick(object? sender, RoutedEventArgs e)
    {
        await DialogManager.ShowOverlayModal(form: ctx => new ProfileForm(this._agendaCore) {DataContext = ctx});
        if (this._viewPresenter.CurrentView == "home")
        {
            HomeView view = (HomeView)this._viewPresenter.Content;
            await view.LoadProfileList();
        }
    }
}