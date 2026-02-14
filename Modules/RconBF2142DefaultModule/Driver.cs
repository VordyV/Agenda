using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Agenda.Core;

namespace Agenda.Modules.RconBF2142DefaultModule;

public class Driver : BasicDriver
{

    public Driver(string id, string moduleId, Dictionary<string, object?> fields) : base(id, moduleId, fields)
    {
    }

    public override async Task OnStart(InitContext ctx)
    {
        Console.WriteLine("1");
    }

    public override async Task OnStop()
    {
        Console.WriteLine("2");
    }

    public override async Task OnLoop()
    {
        Console.WriteLine("3");
        while (!this.Token.IsCancellationRequested)
        {
            await Task.Delay(1);
        }
        Console.WriteLine("4");
    }
}