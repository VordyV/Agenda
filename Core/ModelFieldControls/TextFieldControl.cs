using Avalonia.Controls;

namespace Agenda.Core.ModelFieldControls;

public class TextFieldControl : BaseModelFieldControl
{
    private TextBox _textBox;
    
    public TextFieldControl()
    {
        _textBox = new TextBox();
        this.Content = _textBox;
    }
    
    public override object? GetValue() => string.IsNullOrWhiteSpace(this._textBox.Text) ? null : this._textBox.Text;

    public override void SetValue(object value) => this._textBox.Text = (string)value;
    
    public override Control GetControl() => this._textBox;
}