using Agenda.App.Models;
using Agenda.App.Views;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Agenda.App.Models;

public class MainWindowViewModel : ObservableObject
{
    private Manager _manager;
    private PresenterView _presenterView;

    public MainWindowViewModel(Manager manager)
    {
        _manager = manager;
        _presenterView = new PresenterView(_manager);
        _presenterView.Add("servers", (m, v, a) => new ServersView() {DataContext = new ServersViewModel(m, v)});
        _presenterView.Add("server", (m, v, a) => new ServerView() {DataContext = new ServerViewModel(m, v)});
        _presenterView.LoadNew("servers");
    }

    public ContentControl? CurrentView => _presenterView.View;
}