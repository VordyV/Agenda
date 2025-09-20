using Avalonia.Controls;

namespace Agenda.App;

public interface IDriverField
{
    string Name { get; }
    Control GetControl();
    string? GetValue();
    void SetValue(object? value);
    bool Validate(string value);
}