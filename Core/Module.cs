using System;
using System.Collections.Generic;
using Avalonia.Controls;

namespace Agenda.Core;

public class Module
{
    public string Id { get; private set; }
    public string Title { get; private set; }
    public string Version { get; private set; }
    public Func<UserControl> View { get; private set; }
    public Dictionary<string, ModuleField> Fields;
    public Func<string, Dictionary<string, object?>, BasicDriver> Driver { get; private set; }

    public Module(string id, string title, string version, Func<UserControl> view, List<ModuleField> fields, Func<string, Dictionary<string, object?>, BasicDriver> driver)
    {
        this.Id = id;
        this.Title = title;
        this.Version = version;
        this.View = view;
        this.Fields = this._getDictModuleFields(fields);
        this.Driver = driver;
    }

    private Dictionary<string, ModuleField> _getDictModuleFields(List<ModuleField> moduleFields)
    {
        Dictionary<string, ModuleField> result = new(); 
        foreach (var mf in moduleFields)
        {
            result.Add(mf.Id, mf);
        }
        return result;
    }
}