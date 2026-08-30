using System;
using System.Collections.Generic;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Agenda.Core;

public class Module
{
    public string Id { get; private set; }
    public string Title { get; private set; }
    public string Version { get; private set; }
    public string Description { get; private set; }
    public Func<Connection, BasicView> View { get; private set; }
    public Dictionary<string, ModuleField> Fields;
    public Type Driver { get; private set; }
    public string SubtitleFormat { get; private set; }
    public bool Preview { get; private set; }
    public byte NumberPreviewFields { get; private set; }

    public Module(string id, string title, string version, string description, Func<Connection, BasicView> view, List<ModuleField> fields, Type driver, string subtitleFormat, bool preview, byte numberPreviewFields)
    {
        this.Id = id;
        this.Title = title;
        this.Version = version;
        this.Description = description;
        this.View = view;
        this.Fields = this._getDictModuleFields(fields);
        this.Driver = driver;
        this.SubtitleFormat = subtitleFormat;
        this.Preview = preview;
        this.NumberPreviewFields = numberPreviewFields;
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

    public string GetSubtitle(Dictionary<string, string?> fields)
    {
        string result = this.SubtitleFormat;
        foreach (var field in fields)
        {
            result = result.Replace("{"+field.Key+"}", field.Value);
        }
        return result;
    }

    public BasicDriver GetDriverInstance(string connectionId)
    {
        BasicDriver? driver = Activator.CreateInstance(this.Driver, connectionId) as BasicDriver;
        if (driver == null) throw new Exception("failed to convert the object to Driver");
        return driver;
    }
}