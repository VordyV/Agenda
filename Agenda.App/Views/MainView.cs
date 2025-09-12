using Avalonia.Controls;

namespace Agenda.App.Views;

public class MainView : UserControl
{
    private Manager _manager;
    private PresenterView _presenterView;
    
    public MainView(Manager manager, PresenterView presenterView)
    {
        _manager = manager;
        _presenterView = presenterView;
        
        Content = new TextBlock() { Text = "125" };
    }
}