using System;
using Agenda.Core;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Agenda.Modules.RconBF2142ASModule;

public partial class RconBF2142ASModuleView : BasicView
{
    public RconBF2142ASModuleView(Connection c) : base(c)
    {
        InitializeComponent();
    }

    public RconBF2142ASModuleView()
    {
        InitializeComponent();
    }
    
}