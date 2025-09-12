using Avalonia.Controls;

namespace Agenda.App;

public interface IDriverField
{
    string Name { get; }
    Control GetControl();
    string? GetValue();
    bool Validate(string value);
}