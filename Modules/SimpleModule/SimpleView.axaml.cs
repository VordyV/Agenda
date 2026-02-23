using System;
using Agenda.Core;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Agenda.Modules.SimpleModule;

public partial class SimpleView : BasicView
{
    public SimpleView(Connection c) : base(c)
    {
        InitializeComponent();
    }

    public SimpleView()
    {
        InitializeComponent();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.Conn.Driver is not null)
        {
            Console.WriteLine(1);
        }
    }
}