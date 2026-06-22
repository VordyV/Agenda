using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Agenda.Modules.RconBF2142DefaultModule;

namespace Agenda.Modules.RconBF2142AS;

public enum OperationStatus
{
    Created,
    InProgress,
    Success,
    Error,
    Cancelled
}

public class Operation
{
    public string Id { get; }
    public string Sender { get; }
    public Func<Task<object>> Method { get; }
    public object? Result { get; private set; }
    public OperationStatus Status { get; private set; }
    public Exception? Error { get; private set; }
    public string Description { get; private set; }

    public Operation(string sender, Func<Task<object>> method, string description = "")
    {
        Guid guid = Guid.NewGuid();
        this.Id = guid.ToString("N");
        this.Sender = sender;
        this.Method = method;
        this.Description = description;
    }

    public void SetResult(object result)
    {
        this.Result = result;
    }

    public void SetStatus(OperationStatus status)
    {
        this.Status = status;
    }
    
    public void SetError(Exception error)
    {
        this.Error = error;
    }
}

public class TaskManager
{
    public ConcurrentDictionary<string, Operation> Operations { get; private set; } = new();
    
    public event Action<Operation> OnChangeStatus;
    
    private ConcurrentQueue<Operation> _queue = new();
    private CancellationTokenSource _cts = new CancellationTokenSource();

    public void Enqueue(string sender, Func<Task<object>> method, string description = "")
    {
        Operation operation = new Operation(sender: sender, method: method, description: description);
        this.Operations.TryAdd(operation.Id, operation);
        this.SetStatus(operation, OperationStatus.Created);
        this._queue.Enqueue(operation);
    }

    public void Start()
    {
        Task.Run(()=>this.Loop());
    }

    protected Operation? Get()
    {
        Operation? operation;
        if (this._queue.TryDequeue(out operation)) return operation;
        else return null;
    }

    protected async Task Loop()
    {
        Operation? operation;
        while (!this._cts.IsCancellationRequested)
        {
            operation = this.Get();
            if (operation == null) continue;
            try
            {
                this.SetStatus(operation, OperationStatus.InProgress);
                var result = await operation.Method.Invoke();
                operation.SetResult(result);
                this.SetStatus(operation, OperationStatus.Success);
            }
            catch (Exception error)
            {
                operation.SetError(error);
                this.SetStatus(operation, OperationStatus.Error);
            }
        }
    }

    public async Task Close()
    {
        await this._cts.CancelAsync();
        this._cts.Dispose();
        
        Operation? operation;
        bool exists = this._queue.TryDequeue(out operation);
        while (exists)
        {
            this.SetStatus(operation, OperationStatus.Cancelled);
            exists = this._queue.TryDequeue(out operation);
        }
    }

    public void SetStatus(Operation operation, OperationStatus status)
    {
        operation.SetStatus(status);
        this.OnChangeStatus.Invoke(operation);
        //if (this.OnChangeStatus != null) this.OnChangeStatus.Invoke(operation);
    }
}