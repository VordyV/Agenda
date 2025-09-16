using Agenda.App.Models;
using Agenda.App.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace Agenda.App;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Manager manager = new Manager();
            desktop.MainWindow = new MainWindowView()
            {
                DataContext = new MainWindowViewModel(manager: manager)
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}