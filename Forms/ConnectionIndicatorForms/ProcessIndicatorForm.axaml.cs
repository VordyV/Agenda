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
    public ProcessIndicatorForm()
    {
        InitializeComponent();
    }

    public void SetStatus(string text) => this.TextBlockStatus.Text = text;
    public void SetText(string text) => this.TextBlockText.Text = text;
}