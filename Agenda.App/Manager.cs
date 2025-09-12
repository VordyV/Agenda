using System;
using System.Collections.Generic;

namespace Agenda.App;

public class Manager
{
    public Dictionary<string, Driver> Drivers = new ();
    
    public Manager()
    {
        
    }

    public void AddDriver(string name, Driver driver) => Drivers.Add(name, driver);
}