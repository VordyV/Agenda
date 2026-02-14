using Avalonia.Controls;

namespace Agenda.Core.ModelFieldControls;

public class PasswordFieldControl : BaseModelFieldControl
{
    private TextBox _textBox;
    
    public PasswordFieldControl()
    {
        _textBox = new TextBox() {Classes = { "revealPasswordButton" }};
        this.Content = _textBox;
    }
    
    public override object? GetValue() => string.IsNullOrWhiteSpace(this._textBox.Text) ? null : this._textBox.Text;

    public override void SetValue(object value) => this._textBox.Text = (string)value;
    
    public override Control GetControl() => this._textBox;
}