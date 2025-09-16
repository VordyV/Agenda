using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Agenda.App.Models;

public partial class ServerViewModel : ObservableObject
{
    private Manager _manager;
    private PresenterView _presenterView;
    
    public ServerViewModel(Manager manager, PresenterView presenterView)
    {
        _manager = manager;
        _presenterView = presenterView;
    }

    [RelayCommand]
    public void GoServers() => _presenterView.LoadNew("servers");
}