using System.Collections.Generic;

namespace Agenda.Modules.RconBF2142DefaultModule;

public class RconTask
{
    public string Name { get; private set; }
    public string Command { get; private set; }
    public string? Result { get; private set; } = null;

    public Dictionary<string, string>? Details { get; private set; }

    public RconTask(string name, string command, Dictionary<string, string>? details = null)
    {
        this.Name = name;
        this.Command = command;
        this.Details = details;
    }

    public void SetResult(string result) => this.Result = result;
}