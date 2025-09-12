using System.Collections.Generic;
using Agenda.App.Drivers;
using Agenda.App.Windows;
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
            
            Driver driverRconBf2cc = new Driver("RCON BF2CC", () => new Rconbf2cc());
            driverRconBf2cc.AddField("Address", () => new DriverFieldText("address", "Address"));
            driverRconBf2cc.AddField("Port", () => new DriverFieldText("port","Port"));
            driverRconBf2cc.AddField("Password", () => new DriverFieldText("password","Password"));
            
            Driver driverDaemonBf2cc = new Driver("Daemon BF2CC", () => new Rconbf2cc());
            driverDaemonBf2cc.AddField("Address", () => new DriverFieldText("address","Address"));
            driverDaemonBf2cc.AddField("Port", () => new DriverFieldText("port","Port"));
            driverDaemonBf2cc.AddField("Username", () => new DriverFieldText("username","Username"));
            driverDaemonBf2cc.AddField("Password", () => new DriverFieldText("password","Password"));
            
            manager.AddDriver("rconbf2cc", driverRconBf2cc);
            manager.AddDriver("daemonbf2cc", driverDaemonBf2cc);
            
            desktop.MainWindow = new MainWindow(manager);
        }

        base.OnFrameworkInitializationCompleted();
    }
}