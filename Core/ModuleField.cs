using System;
using Agenda.Core.ModelFieldControls;
using Avalonia.Controls;

namespace Agenda.Core;

public class ModuleField
{
    public string Id { get; private set; }
    public string Title { get; private set; }

    public Func<BaseModelFieldControl> Control { get; private set; }
    public Func<object, ValidatorResult>? Validator { get; private set; }
    
    public bool Required { get; private set; }

    public ModuleField(string id, string title, Func<BaseModelFieldControl> control, Func<object, ValidatorResult>? validator = null, bool required = false)
    {
        this.Id = id;
        this.Title = title;
        this.Control = control;
        this.Validator = validator;
        this.Required = required;
    }
}