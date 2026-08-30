namespace Agenda.Core;

public abstract class PreviewField
{
    public string Label { get; set; }
}

public class StatusPreviewField : PreviewField
{
    public string Color { get; set; }
    public string Text { get; set; }
}

public class TextPreviewField : PreviewField
{
    public string Text { get; set; }
}

public class PlayersPreviewField : PreviewField
{
    public ushort MaxNumber { get; set; }
    public ushort CurrentNumber { get; set; }
}