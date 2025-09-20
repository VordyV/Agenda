using System;
using System.Collections.ObjectModel;
using Agenda.App.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Collections;

namespace Agenda.App.Models;

public partial class ServersViewModel : ObservableObject
{
    private Manager _manager;
    private PresenterView _presenterView;

    [ObservableProperty] 
    private ObservableCollection<ServerItem> _servers = new();
    
    public ServersViewModel(Manager manager, PresenterView presenterView)
    {
        _manager = manager;
        _presenterView = presenterView;
        OnClickRefresh();
    }

    [RelayCommand]
    public void GoServer() => _presenterView.LoadNew("server");

    [RelayCommand]
    public void OnClickRefresh()
    {
        Servers.Clear();
        foreach (ServerItem server in _manager.Config.GetAll())
        {
            server.TypeName = _manager.Drivers[server.Type].Name;
            Servers.Add(server);
            Console.WriteLine(server);
        }
    }

    [RelayCommand]
    public void OnClickEdit(ServerItem server)
    {
        _presenterView.LoadNew("server", server);
    }
    
    [RelayCommand]
    public void OnClickRemove(ServerItem server)
    {
        _manager.Config.Delete(server.Id);
        OnClickRefresh();
    }
}