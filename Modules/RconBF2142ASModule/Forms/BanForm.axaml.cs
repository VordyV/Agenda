using System;
using Agenda.Forms;
using Agenda.Modules.RconBF2142AS;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Agenda.Modules.RconBF2142ASModule.Forms;

public partial class BanForm : UserControl
{
    public BanForm()
    {
        InitializeComponent();
    }
    
    public BanForm(string nick)
    {
        InitializeComponent();
        this.TextBlockNick.Text = nick;
    }

    private void ButtonKick_OnClick(object? sender, RoutedEventArgs e)
    {
        TimeSpan? period = null;

        if (this.ComboBoxDuration.SelectedItem is ComboBoxItem item)
        {
            int duration = int.Parse(this.TextBoxDuration.Text ?? "0");
            switch (item.Name)
            {
                case "BoxItemCBDminutes":
                    period = TimeSpan.FromMinutes(minutes: duration);
                    break;
                case "BoxItemCBDhours":
                    period = TimeSpan.FromHours(hours: duration);
                    break;
                case "BoxItemCBDdays":
                    period = TimeSpan.FromDays(days: duration);
                    break;
                case "BoxItemCBDmonths":
                    period = TimeSpan.FromDays(days: 30*duration);
                    break;
            }
        }
        
        if (DataContext is DialogContext ctx) ctx.Close(new Ban() {});
    }
}