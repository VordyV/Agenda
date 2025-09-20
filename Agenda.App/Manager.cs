using System.Collections.Generic;
using Agenda.App.Services;

namespace Agenda.App;

public class Manager
{
    public Dictionary<string, Driver> Drivers = new ();
    public ConfigService Config;

    public Manager(ConfigService config)
    {
        Config = config;
    }
    
    public void AddDriver(string name, Driver driver) => Drivers.Add(name, driver);
}