namespace Agenda.Core;

public class ManifestFields
{
    public string id { get; set; }
    public string title { get; set; }
    public string type { get; set; }
    public bool required { get; set; }
    public string? value { get; set; }
    public int? length { get; set; }
    public int? max_number { get; set; }
    public int? min_number { get; set; }
}

public class ManifestTags
{
    public string color { get; set; }
    public string text { get; set; }
}

public class ModuleManifest
{
    public string id { get; set; }
    public string title { get; set; }
    public string version { get; set; }
    public string description { get; set; }
    public string view { get; set; }
    public string target { get; set; }
    public byte type { get; set; }
    public List<ManifestFields> fields { get; set; }
    public string driver { get;  set; }
    public string subtitle_format { get; set; }
    public bool preview { get; set; }
    public byte number_preview_fields { get; set; }
    public List<ManifestTags> tags { get; set; }
}