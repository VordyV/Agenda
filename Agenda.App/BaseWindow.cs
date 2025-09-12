using Avalonia.Controls;

namespace Agenda.App;

public abstract class BaseWindow : Window
{
    public Manager Manager { get; set; }
    public PresenterView PresenterView { get; set; }
    
    public abstract void SetupUi();
    
    public BaseWindow(Manager manager)
    {
        Manager = manager;
        PresenterView = new PresenterView(manager: Manager);
        SetupUi();
    }
}