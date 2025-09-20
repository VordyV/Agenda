using Avalonia.Controls;

namespace Agenda.App;

public class DriverFieldText : IDriverField
{
    private TextBox _control;

    public DriverFieldText(string name, string label)
    {
        Name = name;
        _control = new TextBox(){Watermark = label};
    }

    public string? GetValue() => _control.Text;

    public void SetValue(object? value) => _control.Text = value == null ? null : (string)value;

    public string Name { get; }
    public Control GetControl() => _control;

    public bool Validate(string? value)
    {
        return true;
    }
}