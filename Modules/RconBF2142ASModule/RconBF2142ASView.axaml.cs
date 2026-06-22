using System;
using Agenda.Controls;
using Agenda.Core;
using Agenda.Modules.RconBF2142ASModule.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Ursa.Controls;

namespace Agenda.Modules.RconBF2142AS;

public partial class RconBF2142ASView: BasicView
{
    protected ViewPresenter _viewPresenter;
    
    public RconBF2142ASView()
    {
        InitializeComponent();
    }
    
    public RconBF2142ASView(Connection c) : base(c)
    {
        this._viewPresenter = new ViewPresenter(
            views: new()
            {
                {"console", (m, p, a) => new ConsoleView(conn: (Connection)a)},
                {"players", (m, p, a) => new PlayersView(conn: (Connection)a)},
                {"logs", (m, p, a) => new LogsView(conn: (Connection)a, presenter: p)},
            }
        );
        
        this._viewPresenter.OnShowView += this._onShowView;
        InitializeComponent();
        this._viewPresenter.ShowView("console", this.Conn);
    }
    
    private void _onShowView(string view)
    {
        this.MainContent.Content = this._viewPresenter.Content;
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.Conn.Driver is RconBF2142ASDriver driver)
        {
            driver.TaskManager.Enqueue("console", async () => await driver.RconClient.Invoke("exec sv.servername"));
        }
    }

    private void NavMenu_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (this.NavMenu.SelectedItem is NavMenuItem item)
        {
            string name = item.Name.Split('_')[1].ToLower();
            this._viewPresenter.ShowView(name, this.Conn);
        }
        
        //this._viewPresenter.ShowView();
    }
}