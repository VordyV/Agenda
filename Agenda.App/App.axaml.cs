using Agenda.App.Drivers;
using Agenda.App.Models;
using Agenda.App.Services;
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
            ConfigService config = new ConfigService("Agenda.db");
            Manager manager = new Manager(config: config);

            Driver dRconBf2cc = new Driver("RCON BF2CC", () => new Rconbf2cc());
            dRconBf2cc.AddField("Address", () => new DriverFieldText("address", "Address", minLength: 7, maxLength: 15));
            dRconBf2cc.AddField("Port", () => new DriverFieldText("port","Port", onlyNumbers: true, maxNumber: 65535));
            dRconBf2cc.AddField("Password", () => new DriverFieldText("password","Password", minLength: 1, maxLength: 100));
            
            Driver dDaemonBf2cc = new Driver("Daemon BF2CC", () => new Rconbf2cc());
            dDaemonBf2cc.AddField("Address", () => new DriverFieldText("address", "Address", minLength: 7, maxLength: 15));
            dDaemonBf2cc.AddField("Port", () => new DriverFieldText("port","Port", onlyNumbers: true, maxNumber: 65535));
            dDaemonBf2cc.AddField("Username", () => new DriverFieldText("username","Username", minLength: 1, maxLength: 30));
            dDaemonBf2cc.AddField("Password", () => new DriverFieldText("password","Password", minLength: 1, maxLength: 100));
            
            manager.AddDriver("rconbf2cc", dRconBf2cc);
            manager.AddDriver("daemonbf2cc", dDaemonBf2cc);
            
            desktop.MainWindow = new MainWindowView()
            {
                DataContext = new MainWindowViewModel(manager: manager)
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}