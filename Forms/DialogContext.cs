using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Irihi.Avalonia.Shared.Contracts;

namespace Agenda.Forms;

public partial class DialogContext : ObservableObject, IDialogContext  
{  
    public void Close()  
    {  
        RequestClose?.Invoke(this, true);  
    }

    public void Close(bool result)
    {
        RequestClose?.Invoke(this, result);
    }
  
    public event EventHandler<object?>? RequestClose;  
}