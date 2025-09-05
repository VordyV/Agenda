using System;
using Agenda.App.Views;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Interactivity;
using Ursa.Controls;

namespace Agenda.App.Windows;

public partial class MainWindow : Window
{
    private Manager _manager;
    private PresenterView _presenterView;

    private Menu _uiMenu;
    private MenuItem _uiMenuItem_Servers;
    private Grid _uiGridMain;

    private Control LoadUi()
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
        Grid.SetRow(_presenterView.View, 1);
        
        _uiGridMain.Children.Add(_uiMenu);
        _uiGridMain.Children.Add(_presenterView.View);

        _uiMenuItem_Servers.Click += (_, _) => _presenterView.LoadNew("servers");

        return _uiGridMain;
    }

    public MainWindow(Manager manager)
    {
        _manager = manager;
        _presenterView = new PresenterView(manager: _manager);
        _presenterView.Add("main", m => new MainView(m));
        _presenterView.Add("servers", m => new ServersView(m));

        _presenterView.LoadNew("main");
        
        Content = LoadUi();
    }
}