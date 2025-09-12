using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;

namespace Agenda.App.Controls.Buttons;

public class CButton : UserControl
{
    private TextBlock _uiTextBlock;
    public Button Control;
    
    public CButton(string? text, string theme = "SolidButton", string type = "Primary", Action<RoutedEventArgs>? onClick = null)
    {
        _uiTextBlock = new TextBlock() { Text = text };
        Control = new Button() { Content = _uiTextBlock, Theme = (ControlTheme)Application.Current.FindResource(theme), Classes = {type}};
        Control.Click += (sender, args) => onClick?.Invoke(args);
        Content = Control;
    }

    public string? Text => _uiTextBlock.Text;
}