using System;

namespace Agenda.Modules.RconBF2142DefaultModule;

public enum BanType
{
    Ip,
    Key
}

public class Ban
{
    public string Reason { get; set; }
    public bool Notify { get; set; }
    public TimeSpan? Timeout { get; set; }
    public bool Round { get; set; }
    public bool Perm { get; set; }
    public BanType Type { get; set; }
}