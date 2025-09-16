using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Agenda.App.Models;

public partial class ServersViewModel : ObservableObject
{
    private Manager _manager;
    private PresenterView _presenterView;
    
    public ServersViewModel(Manager manager, PresenterView presenterView)
    {
        _manager = manager;
        _presenterView = presenterView;
    }

    [RelayCommand]
    public void GoServer() => _presenterView.LoadNew("server");
}