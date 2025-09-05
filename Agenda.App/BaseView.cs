using Avalonia.Controls;

namespace Agenda.App;

public abstract class BaseView : UserControl
{
    public Manager Manager { get; set; }
    public abstract Control LoadUi();
    
    public BaseView(Manager manager)
    {
        Manager = manager;
        Content = LoadUi();
    }
}