using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Agenda.Core;
using rconnet.RconBf2142.Default;

namespace Agenda.Modules.RconBF2142AS;

public class RconBF2142ASDriver : BasicDriver
{
    public DefaultClient? RconClient;
    public TaskManager TaskManager = new TaskManager();
    
    public RconBF2142ASDriver(string connId) : base(connId) { }
    
    public override async Task OnStart(InitContext ctx, Dictionary<string, object?> fields)
    {
        IPAddress addr = (IPAddress)fields["address"];
        ushort port = Convert.ToUInt16((int)fields["rcon_port"]);
        string password = (string)fields["rcon_password"];
        
        this.RconClient = new DefaultClient(address: addr.ToString(), password: password, port: port);
        await this.RconClient.Start();
        this.TaskManager.Start();

        //this.TaskManager.OnChangeStatus += (op) => { Console.WriteLine($"status = {op.Status}, sender = {op.Sender}, result = {op.Result}, error = {op.Error?.Message}"); };
    }
    
    public override async Task OnStop()
    {
        await this.TaskManager.Close();
        this.RconClient?.Dispose();
    }
    
    public override async Task OnLoop()
    {
        while (!this.Token.IsCancellationRequested)
        {
            await Task.Delay(1000, Token);
        }
    }
}