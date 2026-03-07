using Agenda.Forms;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Irihi.Avalonia.Shared.Contracts;

namespace Agenda.Modules.RconBF2142DefaultModule.Forms;

public partial class KickForm : UserControl
{
    public KickForm()
    {
        InitializeComponent();
    }


    private void ButtonKick_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is DialogContext ctx) ctx.Close(this.AutoCompleteBoxReason.Text ?? "");
    }
}