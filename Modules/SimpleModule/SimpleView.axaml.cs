using System;
using Agenda.Core;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Agenda.Modules.SimpleModule;

public partial class SimpleView : UserControl
{
    public SimpleView()
    {
        InitializeComponent();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is SimpleViewModel vm)
        {
            vm.Test();
        }
        //Console.WriteLine(this._driver.Test());
    }
}