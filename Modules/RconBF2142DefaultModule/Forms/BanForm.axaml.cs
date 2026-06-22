using System;
using Agenda.Forms;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ExCSS;

namespace Agenda.Modules.RconBF2142DefaultModule.Forms;

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
        TimeSpan? timeout = null;

        if (this.ComboBoxDuration.SelectedItem is ComboBoxItem item)
        {
            int duration = int.Parse(this.TextBoxDuration.Text ?? "0");
            switch (item.Name)
            {
                case "BoxItemCBDminutes":
                    timeout = TimeSpan.FromMinutes(minutes: duration);
                    break;
                case "BoxItemCBDhours":
                    timeout = TimeSpan.FromHours(hours: duration);
                    break;
                case "BoxItemCBDdays":
                    timeout = TimeSpan.FromDays(days: duration);
                    break;
                case "BoxItemCBDmonths":
                    timeout = TimeSpan.FromDays(days: 30*duration);
                    break;
            }
        }
        
        Ban ban = new Ban()
        {
            Reason = this.AutoCompleteBoxReason.Text ?? "",
            Notify = this.CheckBoxNop.IsChecked.GetValueOrDefault(false),
            Timeout = timeout,
            Round = this.RadioButtonRound.IsChecked.GetValueOrDefault(false),
            Perm = this.RadioButtonPerm.IsChecked.GetValueOrDefault(false),
            Type = this.RadioButtonAddress.IsChecked.GetValueOrDefault(false) ? BanType.Ip : BanType.Key
            
        };
        if (DataContext is DialogContext ctx) ctx.Close(ban);
    }

    private void CheckBoxNop_OnClick(object? sender, RoutedEventArgs e)
    {
        this.AutoCompleteBoxReason.IsEnabled = !this.AutoCompleteBoxReason.IsEnabled;
    }
}