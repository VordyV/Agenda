using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Agenda.Core;

public abstract class BasicDriver
{
    public string Id { get; private set; }
    public string ModuleId { get; private set; }
    public Dictionary<string, object?> Fields { get; private set; }

    public DriverState State { get; private set; } = new DriverState()
    {
        Type = TypeDriverState.Created
    };
    public event Action<DriverState>? StateChanged;
    public bool Connected { get; private set; } = false;

    private CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();
    public CancellationToken Token => this._cancelTokenSource.Token;

    public BasicDriver(string id, string moduleId, Dictionary<string, object?> fields)
    {
        this.Id = id;
        this.ModuleId = moduleId;
        this.ModuleId = moduleId;
        this.Fields = fields;
    }
    
    public virtual async Task OnStart(InitContext ctx) {}
    public virtual async Task OnStop() {}
    
    public virtual async Task OnLoop() {}

    public void SetState(DriverState? state = null, bool? connected = null)
    {
        if (state == null & connected == null) throw new Exception("At least one argument must not be null");
        if (connected != null) this.Connected = connected.Value;
        if (state != null)
        {
            this.State = state;
            StateChanged?.Invoke(state);
        }
    }

    public void Cancel() => this._cancelTokenSource.Cancel();

    public void Dispose()
    {
        this._cancelTokenSource.Dispose();
    }
}