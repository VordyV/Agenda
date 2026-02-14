using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Agenda.Core;

namespace Agenda.Modules.SimpleModule;

public class Driver : BasicDriver
{

    public Driver(string id, Dictionary<string, object?> fields) : base(id, fields)
    {
    }

    public override async Task OnStart()
    {
        Console.WriteLine("1");
        await Task.Delay(3000);
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