namespace Agenda.Modules.RconBF2142DefaultModule;

public class RconTask
{
    public string Name { get; private set; }
    public string Command { get; private set; }
    public string? Result { get; private set; } = null;

    public RconTask(string name, string command)
    {
        this.Name = name;
        this.Command = command;
    }

    public void SetResult(string result) => this.Result = result;
}