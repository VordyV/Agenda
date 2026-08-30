using System;

namespace Agenda.Core;

public abstract class EventArgs { }

public class BaseEventArgs : EventArgs { public string ConnectionId { get; set; } }

public class CreateConnEventArgs : BaseEventArgs { }

public class InitConnEventArgs : BaseEventArgs { public InitContext Context { get; set; } }

public class ChangeStatusConnEventArgs : BaseEventArgs { public DriverState? State { get; set; } public bool? IsConnected; }

public class StopConnConnEventArgs : BaseEventArgs { }

public class ErrorEventArgs : EventArgs { public Exception Error { get; set; } }