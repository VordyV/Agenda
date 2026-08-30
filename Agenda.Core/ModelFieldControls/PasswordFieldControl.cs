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
    
    public override string? GetValue() => this._textBox.Text;

    public override void SetValue(string? value) => this._textBox.Text = value;
    
    public override Control GetControl() => this._textBox;
}