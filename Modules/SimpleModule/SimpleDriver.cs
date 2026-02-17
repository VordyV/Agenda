using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Agenda.Core;

namespace Agenda.Modules.SimpleModule;

public class SimpleDriver : BasicDriver
{
    public bool Test()
    {
        return true;
    }

    public override async Task OnStart(InitContext ctx, Dictionary<string, object?> fields)
    {
        Console.WriteLine("1");
        ctx.Action("1", "11");
        await Task.Delay(1000, this.Token);
        ctx.Action("2", "22");
        await Task.Delay(1000, this.Token);
        //throw new InitException("rA9", "Connor didn't show up to Amanda's");
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