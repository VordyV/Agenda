using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Agenda.Core;
using Agenda.Forms;
using Agenda.Forms.ConnectionIndicatorForms;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Ursa.Controls;

namespace Agenda;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        AgendaCore agendaCore = new AgendaCore() {PluginEntryPoint = Settings.PluginEntryPoint, PluginsDir = Settings.PluginsDir};
        agendaCore.RegisterModules(Settings.Modules);
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            #if !DEBUG
            SplashScreen splashScreen = new SplashScreen();
            desktop.MainWindow = splashScreen;
            splashScreen.Show();

            await splashScreen.Run();
            #endif
            
            MainWindow mainWindow = new MainWindow(agendaCore);
            desktop.MainWindow = mainWindow;
            desktop.MainWindow.Closing += async (sender, args) => await agendaCore.Dispose();
            mainWindow.Show();
            
            #if !DEBUG
            splashScreen.Close();
            #endif
        }

        base.OnFrameworkInitializationCompleted();
    }
}