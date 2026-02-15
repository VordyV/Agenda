using System;
using System.Threading;
using Agenda.Core;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Irihi.Avalonia.Shared.Contracts;

namespace Agenda.Forms.ConnectionIndicatorForms;

public partial class ProcessIndicatorForm : UserControl
{
    private Action<object?, RoutedEventArgs> _onClickCancel;
    
    public ProcessIndicatorForm()
    {
        InitializeComponent();
    }
    
    public ProcessIndicatorForm(Action<object?, RoutedEventArgs> onClickCancel)
    {
        this._onClickCancel += onClickCancel;
        InitializeComponent();
    }

    public void SetStatus(string text) => this.TextBlockStatus.Text = text;
    public void SetText(string text) => this.TextBlockText.Text = text;

    private void ButtonCancel_OnClick(object? sender, RoutedEventArgs e) => this._onClickCancel?.Invoke(sender, e);
}