using Avalonia.Controls;

namespace Agenda.App.Views;

public class MainView : UserControl
{
    private Manager _manager;
    
    public MainView(Manager manager)
    {
        _manager = manager;
        
        Content = new TextBlock() { Text = "125" };
    }
}