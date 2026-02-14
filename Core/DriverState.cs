namespace Agenda.Core;

public class DriverState
{
    public TypeDriverState Type { get; set; }
    public TypeDriverError? Error { get; set; }
    public string? ErrorDetail { get; set; }
}