using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Agenda.Modules.QueryGS3Module;

public class ServerStatus
{
    public byte[] RawData { get; set; } = Array.Empty<byte>();
    public string? ParseError { get; set; }
    public Dictionary<string, string> Info { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public List<Dictionary<string, string>> Players { get; set; } = new();
    public List<Dictionary<string, string>> Teams { get; set; } = new();
    public bool IsParsedSuccessfully => ParseError == null;
    public long Delay { get; set; }
}

public class BFQuery
{
    private readonly string _host;
    private readonly int _port;
    private readonly int _timeoutMs;

    public BFQuery(string host, int port = 29900, int timeoutMs = 5000)
    {
        _host = host;
        _port = port;
        _timeoutMs = timeoutMs;
    }

    public async Task<ServerStatus> GetStatusAsync()
    {
        using var client = new UdpClient();
        client.Client.ReceiveTimeout = _timeoutMs;
        client.Client.SendTimeout = _timeoutMs;

        var remote = new IPEndPoint(IPAddress.Parse(_host), _port);

        byte[] request = new byte[]
        {
            0xFE, 0xFD,
            0x00,
            0x04, 0x05, 0x06, 0x07,
            0xFF, 0xFF, 0xFF, 0x01
        };

        var watch = System.Diagnostics.Stopwatch.StartNew();
        await client.SendAsync(request, remote);

        byte[] fullResponse = await ReadAllPacketsAsync(client);

        var status = new ServerStatus
        {
            RawData = fullResponse
        };

        try
        {
            if (fullResponse.Length > 0)
                ParseResponse(fullResponse, status);
            else
                status.ParseError = "No response from server";
        }
        catch (Exception ex)
        {
            status.ParseError = ex.Message;
        }

        watch.Stop();
        status.Delay = watch.ElapsedMilliseconds;

        return status;
    }

    private async Task<byte[]> ReadAllPacketsAsync(UdpClient client)
    {
        var payloads = new Dictionary<int, byte[]>();
        int packetCount = -1;

        using var cts = new CancellationTokenSource(_timeoutMs);

        while (packetCount == -1 || payloads.Count < packetCount)
        {
            UdpReceiveResult result;

            try
            {
                result = await client.ReceiveAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (SocketException)
            {
                break;
            }
            
            cts.CancelAfter(_timeoutMs);

            byte[] response = result.Buffer;
            if (response.Length < 16 || response[0] != 0x00)
                continue;

            using var ms = new MemoryStream(response);
            using var br = new BinaryReader(ms);

            br.ReadByte();
            br.ReadBytes(13);

            byte numPackets = br.ReadByte();
            int number = numPackets & 0x7F;

            if ((numPackets & 0x80) != 0)
                packetCount = number + 1;

            byte objId = br.ReadByte();
            byte[] header = Array.Empty<byte>();

            if (objId >= 1)
            {
                string key = ReadNullTerminatedString(br);
                byte count = br.ReadByte();

                if (count == 0)
                {
                    var headerList = new List<byte> { 0x00, objId };
                    headerList.AddRange(Encoding.UTF8.GetBytes(key));
                    headerList.Add(0x00);
                    headerList.Add(0x00);
                    header = headerList.ToArray();
                }
            }

            byte[] remaining = br.ReadBytes((int)(ms.Length - ms.Position));
            if (remaining.Length > 0)
                remaining = remaining[..^1];

            byte[] payload = new byte[header.Length + remaining.Length];
            Buffer.BlockCopy(header, 0, payload, 0, header.Length);
            Buffer.BlockCopy(remaining, 0, payload, header.Length, remaining.Length);

            int lastNull = Array.LastIndexOf(payload, (byte)0x00);
            if (lastNull >= 0)
                payload = payload[..(lastNull + 1)];

            payloads[number] = payload;
        }

        if (payloads.Count == 0)
            return Array.Empty<byte>();

        var ordered = new List<byte>();
        foreach (var key in payloads.Keys.OrderBy(k => k))
            ordered.AddRange(payloads[key]);

        return ordered.ToArray();
    }

    private void ParseResponse(byte[] data, ServerStatus status)
    {
        using var ms = new MemoryStream(data);
        using var br = new BinaryReader(ms);
        
        while (true)
        {
            string key = ReadNullTerminatedString(br);
            if (string.IsNullOrEmpty(key))
                break;

            string value = ReadNullTerminatedString(br);
            status.Info[key] = value;
        }
        
        status.Players = GetDictionaries(br, "player");
        
        status.Teams = GetDictionaries(br, "team");
    }

    private List<Dictionary<string, string>> GetDictionaries(BinaryReader br, string objectType)
    {
        var list = new List<Dictionary<string, string>>();

        if (br.BaseStream.Position >= br.BaseStream.Length)
            return list;

        br.ReadByte();

        int i = 0;

        while (br.BaseStream.Position < br.BaseStream.Length)
        {
            string key = ReadNullTerminatedString(br);
            if (string.IsNullOrEmpty(key))
                break;

            br.ReadByte();

            key = key.TrimEnd('t').TrimEnd('_');

            if (key == objectType)
                key = "name";

            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                string value = ReadNullTerminatedString(br).Trim();

                if (string.IsNullOrEmpty(value))
                    break;

                if (list.Count <= i)
                    list.Add(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));

                list[i][key] = value;
                i++;
            }

            i = 0;
        }

        return list;
    }

    private static string ReadNullTerminatedString(BinaryReader br)
    {
        var bytes = new List<byte>();
        while (br.BaseStream.Position < br.BaseStream.Length)
        {
            byte b = br.ReadByte();
            if (b == 0x00)
                break;
            bytes.Add(b);
        }
        return Encoding.UTF8.GetString(bytes.ToArray());
    }
}