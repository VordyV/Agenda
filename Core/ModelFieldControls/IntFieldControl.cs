using Avalonia.Controls;

namespace Agenda.Core.ModelFieldControls;

public class IntFieldControl : BaseModelFieldControl
{
    private NumericUpDown _numericUpDown;
    
    public IntFieldControl(decimal max = 2147483647.0m, decimal min = 0.0m)
    {
        _numericUpDown = new NumericUpDown() {ShowButtonSpinner=false, Maximum = max, Minimum = min};
        this.Content = _numericUpDown;
    }

    public override string? GetValue()
    {
        if (this._numericUpDown.Value == null) return null;
        return this._numericUpDown.Value.ToString();
    }

    public override void SetValue(string? value) => this._numericUpDown.Value = int.Parse(value);

    public override Control GetControl() => this._numericUpDown;
}