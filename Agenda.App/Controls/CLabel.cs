using Avalonia.Controls;

namespace Agenda.App.Controls;

public class CLabel : UserControl
{
    private TextBlock _uiTextBlock;
    
    public CLabel(string text)
    {
        _uiTextBlock = new TextBlock() { Text = text };
        Content = _uiTextBlock;
    }

    public string? Text => _uiTextBlock.Text;
}