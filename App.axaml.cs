using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Agenda.Core;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace Agenda;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        Manager manager = new Manager();
        manager.RegisterModules(Settings.Modules);
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow(manager);
        }

        base.OnFrameworkInitializationCompleted();
    }
}