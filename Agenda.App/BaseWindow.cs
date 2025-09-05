using Avalonia.Controls;

namespace Agenda.App;

public abstract class BaseWindow : Window
{
    public Manager Manager { get; set; }
    public PresenterView PresenterView { get; set; }
    
    public abstract Control LoadUi();
    
    public BaseWindow(Manager manager)
    {
        Manager = manager;
        PresenterView = new PresenterView(manager: Manager);
        Content = LoadUi();
    }
}