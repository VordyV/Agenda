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
        AgendaCore agendaCore = new AgendaCore();
        agendaCore.RegisterModules(Settings.Modules);
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow(agendaCore);
            desktop.MainWindow.Closing += async (sender, args) => await agendaCore.Dispose();
        }

        base.OnFrameworkInitializationCompleted();
    }
}