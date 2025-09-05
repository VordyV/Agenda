using System;
using Agenda.App.Views;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Interactivity;
using Ursa.Controls;

namespace Agenda.App.Windows;

public class MainWindow : BaseWindow
{
    private Menu _uiMenu;
    private MenuItem _uiMenuItem_Servers;
    private Grid _uiGridMain;

    public override Control LoadUi()
    {
        _uiMenuItem_Servers = new MenuItem() { Header = "Servers" };
        _uiMenu = new Menu()
        {
            Items =
            {
                _uiMenuItem_Servers
            }
        };
        _uiGridMain = new Grid()
        {
            RowDefinitions = RowDefinitions.Parse("Auto, *")
        };
        
        Grid.SetRow(_uiMenu, 0);
        Grid.SetRow(this.PresenterView.View, 1);
        
        _uiGridMain.Children.Add(_uiMenu);
        _uiGridMain.Children.Add(this.PresenterView.View);

        _uiMenuItem_Servers.Click += (_, _) => this.PresenterView.LoadNew("servers");
        
        return _uiGridMain;
    }

    public MainWindow(Manager manager) : base(manager)
    {
        this.PresenterView.Add("main", m => new MainView(m));
        this.PresenterView.Add("servers", m => new ServersView(m));
        this.PresenterView.LoadNew("servers");
    }
}