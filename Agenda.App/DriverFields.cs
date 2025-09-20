using System;
using Avalonia.Controls;

namespace Agenda.App;

public class DriverFieldText : IDriverField
{
    private TextBox _control;
    private Action<string>? _validator;
    private int _minLength;
    private int _maxLength;
    private bool _onlyNumbers;
    private int _maxNumber;
    private int _minNumber;

    public DriverFieldText(string name, string label, Action<string>? validator = null, int minLength = 0, int maxLength = 255, bool onlyNumbers = false, int maxNumber = Int32.MaxValue, int minNumber = 0)
    {
        Name = name;
        _validator = validator;
        _minLength = minLength;
        _maxLength = maxLength;
        _onlyNumbers = onlyNumbers;
        _maxNumber = maxNumber;
        _minNumber = minNumber;
        _control = new TextBox(){Watermark = label};
    }

    public string? GetValue() => _control.Text;

    public void SetValue(object? value) => _control.Text = value == null ? null : (string)value;

    public string Name { get; }
    public Control GetControl() => _control;

    public bool Validate()
    {
        if (!_onlyNumbers)
        {
            MinLength();
            MaxLength();
        }
        else
        {
            OnlyNumbers();
            MaxNumber();
            MinNumber();
        }
        
        _validator?.Invoke(GetValue());
        return true;
    }

    private void MinLength()
    {
        if (GetValue() == null || GetValue().Length < _minLength) throw new Exception($"It's too short. Minimum length is {_minLength} characters");
    }
    
    private void MaxLength()
    {
        if (GetValue() == null || GetValue().Length > _maxLength) throw new Exception($"It's too long. Maximum length is {_maxLength} characters");
    }

    private void OnlyNumbers()
    {
        int numericValue;
        if (!int.TryParse(GetValue(), out numericValue)) throw new Exception($"It is not a number");
    }
    
    private void MaxNumber()
    {
        int numericValue;
        int.TryParse(GetValue(), out numericValue);
        
        if (numericValue > _maxNumber) throw new Exception($"Number is too big. Maximum {_maxNumber}");
    }
    
    private void MinNumber()
    {
        int numericValue;
        int.TryParse(GetValue(), out numericValue);
        
        if (numericValue < _minNumber) throw new Exception($"Number is too small. Minimum {_minNumber}");
    }
}