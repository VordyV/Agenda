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

    public override object? GetValue()
    {
        if (this._numericUpDown.Value == null) return null;
        return (int)this._numericUpDown.Value;
    }

    public override void SetValue(object value) => this._numericUpDown.Value = (int)value;

    public override Control GetControl() => this._numericUpDown;
}