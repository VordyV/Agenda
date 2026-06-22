using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Agenda.Controls;

public class FormControl : ContentControl
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<FormControl, string>(nameof(Title));

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    
    public static readonly StyledProperty<object?> FooterProperty =
        AvaloniaProperty.Register<FormControl, object?>(nameof(Footer));

    public object? Footer
    {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }
}