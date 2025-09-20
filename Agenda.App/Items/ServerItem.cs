using System.Collections.Generic;

namespace Agenda.App.Items;

public class ServerItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string TypeName { get; set; }
    public Dictionary<string, string?> Options { get; set; }
}