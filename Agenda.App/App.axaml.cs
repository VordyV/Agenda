using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Agenda.Core;
using Agenda.Forms;
using Agenda.Forms.ConnectionIndicatorForms;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Ookii.CommandLine;
using Ursa.Controls;

namespace Agenda;

[GeneratedParser]
public partial class Args
{
    [CommandLineArgument]
    public bool DevMode { get; set; } = false;
    
    [CommandLineArgument]
    public string ExternalModulesDir { get; set; }
}

public partial class App : Application
{
    private AgendaCore AgendaCore;
    
    public override void Initialize()
    {
        this.AgendaCore = new AgendaCore() { ExternalModulesEntryPoint = Settings.ExternalModulesEntryPoint };
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        Console.WriteLine(2);
        this.AgendaCore.RegisterModules(Settings.Modules);
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Args? args = Args.Parse();
            this.AgendaCore.IsDevMode = args?.DevMode ?? false;
            this.AgendaCore.ExternalModulesDir = args?.ExternalModulesDir != null && args?.DevMode != null ? args?.ExternalModulesDir : Settings.ExternalModulesDir;
            #if !DEBUG
            Version version = Assembly.GetEntryAssembly()?.GetName().Version;
            SplashScreen splashScreen = new SplashScreen(version);
            desktop.MainWindow = splashScreen;
            splashScreen.Show();

            await splashScreen.Run();
            #endif
            
            MainWindow mainWindow = new MainWindow(this.AgendaCore);
            desktop.MainWindow = mainWindow;
            desktop.MainWindow.Closing += async (sender, args) => await this.AgendaCore.Dispose();
            mainWindow.Show();
            
            #if !DEBUG
            splashScreen.Close();
            #endif
        }

        base.OnFrameworkInitializationCompleted();
    }
}