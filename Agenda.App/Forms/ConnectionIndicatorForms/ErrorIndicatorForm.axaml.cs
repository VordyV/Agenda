using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Agenda.Forms.ConnectionIndicatorForms;

public partial class ErrorIndicatorForm : UserControl
{
    private Action<object?, RoutedEventArgs> _onClickOk;
    
    public ErrorIndicatorForm()
    {
        InitializeComponent();
    }
    
    public ErrorIndicatorForm(string title, string text, Action<object?, RoutedEventArgs> onClickOk)
    {
        this._onClickOk += onClickOk;
        InitializeComponent();
        this.TextBlockTitle.Text = title;
        this.TextBlockText.Text = text;
    }


    private void ButtonOk_OnClick(object? sender, RoutedEventArgs e) => this._onClickOk?.Invoke(sender, e);
}