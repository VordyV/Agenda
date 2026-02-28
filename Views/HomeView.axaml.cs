using Agenda.Controls;
using Agenda.Core;
using Agenda.Forms;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Ursa.Controls;
using Ursa.Controls.Options;

namespace Agenda.Views;

public partial class HomeView : UserControl
{
    private Manager _manager;
    private ViewPresenter _presenter;
    
    public HomeView(Manager manager, ViewPresenter presenter)
    {
        this._manager = manager;
        this._presenter = presenter;
        InitializeComponent();
    }

    public HomeView()
    {
        InitializeComponent();
    }

    private async void AddNewProfile_OnClick(object? sender, RoutedEventArgs e)
    {

    }

    private void ButtonNewSession_OnClick(object? sender, RoutedEventArgs e)
    {
        var context = new DialogContext();
        OverlayDialog.ShowCustom(new ConnectForm(this._manager) {DataContext = context}, context, hostId: "main", new OverlayDialogOptions() {CanDragMove = false, CanResize = false});
    }
}