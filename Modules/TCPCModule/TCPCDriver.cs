using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Agenda.Core;

namespace Agenda.Modules.TCPCModule;

public class TCPCDriver : BasicDriver
{
    private Socket _socket;
    private long _lastRequestTime = 0;
    private long _lastResponseTime = 0;
    public event Action<Response> OnRecvData;

    public TCPCDriver(string connId) : base(connId)
    {
        
    }
    
    public async Task<bool> SendData(byte[] data)
    {
        try
        {
            await this._socket.SendAsync(data);
            this._lastRequestTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            return true;
        }
        catch (Exception e)
        {
            this._lastRequestTime = 0;
            return false;
        }
    }

    public override async Task OnStart(InitContext ctx, Dictionary<string, object?> fields)
    {
        this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress addr = (IPAddress)fields["address"];
        int port = (int)fields["port"];
        try
        {
            await this._socket.ConnectAsync(addr, port, this.Token);
        }
        catch (SocketException e)
        {
            throw new InitException("Failed to connect", e.Message);
        }
        catch (Exception)
        {
            throw;
        } 
    }

    public override async Task OnStop()
    {
        await this._socket.DisconnectAsync(false);
    }

    public override async Task OnLoop()
    {
        byte[] responseBytes = new byte[512];
        int bytes;

        while (this._socket.Connected)
        {
            bytes = await this._socket.ReceiveAsync(responseBytes, this.Token);
            this._lastResponseTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var dalay = this._lastResponseTime-this._lastRequestTime;
            Console.WriteLine(this._lastResponseTime);
            Console.WriteLine(this._lastRequestTime);
            Console.WriteLine(this._lastResponseTime-this._lastRequestTime);
            this.OnRecvData?.Invoke(new Response() {Data = responseBytes, Bytes = bytes, Delay = dalay});
            Array.Clear(responseBytes, 0, responseBytes.Length);
        }
    }
}