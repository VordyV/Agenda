using System;
using System.Collections.Generic;

namespace Agenda.App;

public class Driver
{
    public string Name { get; }
    public Func<IDriver> GetDriver { get; }
    public Dictionary<string, Func<IDriverField>> Fields { get; } = new();

    public Driver(string name, Func<IDriver> getDriver)
    {
        Name = name;
        GetDriver = getDriver;
    }

    public void AddField(string name, Func<IDriverField> getFunc) => Fields.Add(name, getFunc);
}