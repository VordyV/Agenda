using System;
using System.Collections.Generic;

namespace Agenda.Core;

public enum ValidatorErrorType
{
    InvalidStringLength, // length
    NumberExceedsAllowedLimit, // maxNum
    NumberBelowAllowedLimit, // minNum
}

public class ValidatorResult
{
    public bool Success { get; set; } = true;
    public List<ValidatorErrorType> Errors { get; set; } = new();

    public void AddError(ValidatorErrorType type)
    {
        if (this.Success) this.Success = false;
        this.Errors.Add(type);
    }
}

public class ModuleFieldValidator
{
    // string
    private int? _length = null;
    
    // int
    private int? _maxNum = null;
    private int? _minNum = null;
    
    public ModuleFieldValidator(int? length = null, int? maxNum = null, int? minNum = null)
    {
        this._length = length;
        this._maxNum = maxNum;
        this._minNum = minNum;
    }

    public ValidatorResult Validate(object data)
    {
        ValidatorResult result = new ValidatorResult();
        
        if (data is string)
        {
            string value = (string)data;
            if (this._length != null && value.Length > this._length)
            {
                result.AddError(ValidatorErrorType.InvalidStringLength);
            }
        }
        else if (data is int)
        {
            int value = (int)data;
            if (this._maxNum != null && value > this._maxNum)
            {
                result.AddError(ValidatorErrorType.NumberExceedsAllowedLimit);
            }
            
            if (this._minNum != null && value < this._minNum)
            {
                result.AddError(ValidatorErrorType.NumberBelowAllowedLimit);
            }
        }
        
        return result;
    }
}