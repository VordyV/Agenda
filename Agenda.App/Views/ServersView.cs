using Avalonia.Controls;

namespace Agenda.App.Views;

public class ServersView : UserControl
{
    private Manager _manager;
    
    public ServersView(Manager manager)
    {
        _manager = manager;
        
        Content = new TextBlock() { Text = "000" };
    }
}