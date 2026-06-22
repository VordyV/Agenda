using System;

namespace Agenda.Modules.RconBF2142AS;

public enum BanType
{
    Round,
    Perm,
    Period
}

public enum BanMethod
{
    Address,
    Key
}

public class Ban
{
    public string Nick { get; set; }
    public string Reason { get; set; }
    public bool Notify { get; set; }
    public BanType Type { get; set; }
    public TimeSpan? Period { get; set; }
    public BanMethod Method { get; set; }
}