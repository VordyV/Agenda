using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Agenda.Core;

namespace Agenda.Modules.RconBF2142DefaultModule;

public class RconBf2142DefaultDriver : BasicDriver
{
    private Socket _socket;
    private Queue<RconTask> _tasks = new();
    
    public Action<RconTask> OnRecv;

    public RconBf2142DefaultDriver(string connId) : base(connId)
    {
        
    }

    public async Task SendAsync(string taskName, string command)
    {
        byte[] data = Encoding.UTF8.GetBytes("\x02"+command+"\n");
        if (this._socket.Connected) await this._socket.SendAsync(data);
        this._tasks.Enqueue(new RconTask(name: taskName, command: command));
    }

    private async Task<string> AwaitData(string endCharacter)
    {
        byte[] responseBytes = new byte[1024];
        int bytes;
        string result = "";
        while (true)
        {
            bytes = await this._socket.ReceiveAsync(responseBytes, this.Token);
            result += Encoding.UTF8.GetString(responseBytes, 0, bytes);
            if (result.EndsWith(endCharacter)) break;
        }
        return result;
    }

    public override async Task OnStart(InitContext ctx, Dictionary<string, object?> fields)
    {
        this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress addr = (IPAddress)fields["address"];
        int port = (int)fields["rcon_port"];
        string password = (string)fields["rcon_password"];
        
        try
        {
            ctx.Action("Connection", "Attempting to establish a connection");
            await Task.Delay(200);
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
        
        string patternSeed = @"### Digest seed:\s*([A-Za-z0-9]{16})";
        
        string welcomeMessage = await this.AwaitData("\n\n");
        
        Match match = Regex.Match(welcomeMessage, patternSeed);
        if (!match.Success) throw new InitException("Authorization failed", "Server gave an unexpected response");
        
        ctx.Action("Authorization", "Attempting to establish a connection");
        await Task.Delay(200);
        string seed = match.Groups[1].Value;
        string digest;
        
        using (MD5 md5 = MD5.Create())
        {
            byte[] bseed = Encoding.UTF8.GetBytes(seed);
            md5.TransformBlock(bseed, 0, bseed.Length, bseed, 0);
            
            byte[] bpwd = Encoding.UTF8.GetBytes(password);
            md5.TransformFinalBlock(bpwd, 0, bpwd.Length);
            
            byte[] hash = md5.Hash;
            digest = BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        await this._socket.SendAsync(Encoding.UTF8.GetBytes($"login {digest}\n"));
        
        if (await this.AwaitData("\n") != "Authentication successful, rcon ready.\n") throw new InitException("Authorization failed", "Invalid password");
    }

    public override async Task OnStop()
    {
        await this._socket.DisconnectAsync(false);
    }

    public override async Task OnLoop()
    {
        byte[] responseBytes = new byte[1024];
        int bytes;
        string value;
        
        while (this._socket.Connected)
        {
            bytes = await this._socket.ReceiveAsync(responseBytes, this.Token);
            value = Encoding.UTF8.GetString(responseBytes, 0, bytes).Replace("\x04", "");
            if (value.Substring(value.Length-1) == "\n") value = value.Remove(value.Length-1);
            var s = this._tasks.TryPeek(out var rt);
            if (s)
            {
                rt.SetResult(value);
                this.OnRecv?.Invoke(rt);
            }
        }
    }
}