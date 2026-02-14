using System;
using Avalonia.Controls;

namespace Agenda.Core.ModelFieldControls;

public abstract class BaseModelFieldControl : UserControl
{
    public virtual object? GetValue()
    {
        throw new NotImplementedException();
    }

    public virtual void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public virtual Control GetControl()
    {
        throw new NotImplementedException();
    }
}