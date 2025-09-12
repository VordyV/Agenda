using System.Collections.Generic;
using System.Collections.ObjectModel;
using Agenda.App.Controls;
using Agenda.App.Controls.Buttons;
using Agenda.App.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using ursa = Ursa.Controls;

namespace Agenda.App.Views;

public class ServersView : BaseView
{
    private Grid _uiGrid_Main;
    private CLabel _uiCLabel_Title;
    private CButton _uiCButton_Add;
    private DataGrid _uiDataGrid_Servers;
    private ObservableCollection<Server> _servers = new();
    
    public override void SetupUi()
    {
        Padding = new Thickness(16);
        
        _uiGrid_Main = new Grid()
        {
            ColumnDefinitions = ColumnDefinitions.Parse("Auto, *, Auto"),
            RowDefinitions = RowDefinitions.Parse("Auto, *")
        };
        _uiCLabel_Title = new CLabel("List of servers");
        _uiCButton_Add = new CButton("Add a server");
        _uiDataGrid_Servers = new DataGrid { AutoGenerateColumns = true, ItemsSource = _servers, };
        
        Grid.SetColumn(_uiDataGrid_Servers, 0);
        Grid.SetRow(_uiDataGrid_Servers, 1);
        Grid.SetColumnSpan(_uiDataGrid_Servers, 3);
        
        Grid.SetColumn(_uiCLabel_Title, 0);
        Grid.SetRow(_uiCLabel_Title, 0);
        
        Grid.SetColumn(_uiCButton_Add, 2);
        Grid.SetRow(_uiCButton_Add, 0);
        
        _uiGrid_Main.Children.Add(_uiDataGrid_Servers);
        _uiGrid_Main.Children.Add(_uiCLabel_Title);
        _uiGrid_Main.Children.Add(_uiCButton_Add);

        _uiCButton_Add.Control.Click += (sender, args) => PresenterView.LoadNew("server");
        
        //_servers.Add(new Server() {Name = "Иван", Type = "RCON", Address = "127.0.0.1", Port = 4711, Id = 1});

        Content = _uiGrid_Main;
    }
    
    public ServersView(Manager manager, PresenterView presenterView) : base(manager, presenterView) { }
}