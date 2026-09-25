using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Agenda.Core;

namespace Agenda.Modules.QueryGS3Module;

public class QueryGS3Driver : BasicDriver
{
    private string _address;
    private int _port;
    
    public bool Test()
    {
        return true;
    }

    public QueryGS3Driver(string connId) : base(connId)
    {
        
    }

    public override async Task OnStart(InitContext ctx, Dictionary<string, string?> fields)
    {
        this._address = fields["address"];
        this._port = int.Parse(fields["query_port"]);
        
        //ctx.Action("1", "11");
        //await Task.Delay(1000, this.Token); 
        //ctx.Action("2", "22");
        //await Task.Delay(1000, this.Token);
        //throw new InitException("rA9", "Connor didn't show up to Amanda's");
    }

    public async Task<long> GetPint()
    {
        try
        {
            Ping pingSender = new Ping();
            PingReply reply = await pingSender.SendPingAsync(this._address, 3000);

            if (reply.Status == IPStatus.Success) return reply.RoundtripTime;
            else return -1;
        }
        catch (Exception e)
        {
            return -1;
        }

    }

    public async Task<ServerStatus?> GetData()
    {
        try
        {
            var query = new BFQuery(this._address, this._port);
            return await query.GetStatusAsync();
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public override async Task OnStop()
    {
        Console.WriteLine("2");
    }

    public override async Task OnLoop()
    {
        while (!this.Token.IsCancellationRequested)
        {
            await Task.Delay(1);
        }
    }

    public override async Task<Preview> OnPreview()
    {
        List<PreviewField> previewFields = new();
        
        await Task.Delay(1000);
        previewFields.Add(new StatusPreviewField() {Label = "Status", Text = "Green", Color = "Green"});
        
        return new Preview() {Fields = previewFields.ToArray(), Color = "green"};
    }
    
    public override async Task<Preview> OnPreviewError()
    {
        List<PreviewField> previewFields = new();
        
        previewFields.Add(new StatusPreviewField() {Label = "Status", Text = "Red", Color = "Red"});
        
        return new Preview() {Fields = previewFields.ToArray(), Color = "red"};
    }
}