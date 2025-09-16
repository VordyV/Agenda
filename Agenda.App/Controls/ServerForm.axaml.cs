using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Agenda.App.Controls;

public class ServerForm : TemplatedControl
{
    
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<ServerForm, string>(nameof(Title), "{Title}");
    
    public static readonly StyledProperty<string> SubTitleProperty =
        AvaloniaProperty.Register<ServerForm, string>(nameof(SubTitle), "{SubTitle}");

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string SubTitle
    {
        get => GetValue(SubTitleProperty);
        set => SetValue(SubTitleProperty, value);
    }
    
}