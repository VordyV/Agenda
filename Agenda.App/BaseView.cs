using Avalonia.Controls;

namespace Agenda.App;

public abstract class BaseView : UserControl
{
    public Manager Manager { get; set; }
    public PresenterView PresenterView { get; set; }
    public abstract void SetupUi();
    
    public BaseView(Manager manager, PresenterView presenterView)
    {
        Manager = manager;
        PresenterView = presenterView;
        SetupUi();
    }
}