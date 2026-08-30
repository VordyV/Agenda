using System;
using Agenda.Controls;
using Agenda.Core;
using Agenda.Modules.RconBF2142DefaultModule.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Ursa.Controls;

namespace Agenda.Modules.RconBF2142DefaultModule;

public partial class RconBf2142DefaultView : BasicView
{
    private ViewPresenter _viewPresenter;
    
    public RconBf2142DefaultView()
    {
        InitializeComponent();
    }
    
    public RconBf2142DefaultView(Connection c) : base(c)
    {
        this._viewPresenter = new ViewPresenter(manager: null, 
            views: new()
            {
                {"nmconsole", (mgr, ptr, arg) => new ConsoleView(presenter: ptr, conn: (Connection)arg)},
                {"nmplayers", (mgr, ptr, arg) => new PlayersView(presenter: ptr, conn: (Connection)arg)}
            }
        );
        this._viewPresenter.OnShowView += this._onShowView;
        
        InitializeComponent();
        
        this._viewPresenter.ShowView("nmconsole", this.Conn);
        //this.MainContent.Content = this._viewPresenter.Content;
        this.NavMenu.SelectedItem = this.nmconsole;
    }

    private async void OnRecv(RconTask rt)
    {
        //Avalonia.Threading.Dispatcher.UIThread.Post(() => { this. });
    } 
    
    private void _onShowView(string view)
    {
        this.MainContent.Content = this._viewPresenter.Content;
    }

    private void NavMenu_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (this.NavMenu.SelectedItem is NavMenuItem item)
        {
            this._viewPresenter.ShowView(item.Name, this.Conn);
            this.TextBlockTitle.Text = (string)item.Header;
        }
    }
}