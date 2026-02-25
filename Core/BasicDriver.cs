using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Agenda.Core;

public abstract class BasicDriver
{
    public string ConnectionId { get; private set; }

    public BasicDriver(string connId)
    {
        this.ConnectionId = connId;
        Debug.WriteLine($"[{this.ConnectionId}] Driver created");
    }

    ~BasicDriver()
    {
        Debug.WriteLine($"[{this.ConnectionId}] Driver deleted");
    }
    
    public DriverState State { get; private set; } = new DriverState()
    {
        Type = TypeDriverState.Created
    };
    public event Action<DriverState>? OnStateChanged;
    public bool Connected { get; private set; } = false;

    private CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();
    public CancellationToken Token => this._cancelTokenSource.Token;
    
    public virtual async Task OnStart(InitContext ctx, Dictionary<string, object?> fields) {}
    public virtual async Task OnStop() {}
    
    public virtual async Task OnLoop() {}

    public void SetState(DriverState? state = null, bool? connected = null)
    {
        if (state == null & connected == null) throw new Exception("At least one argument must not be null");
        if (connected != null) this.Connected = connected.Value;
        if (state != null)
        {
            this.State = state;
            this.OnStateChanged?.Invoke(state);
        }
    }

    public void Cancel() => this._cancelTokenSource.Cancel();

    public void Dispose()
    {
        this._cancelTokenSource.Dispose();
    }
}