using CommunityToolkit.Mvvm.ComponentModel;

namespace Agenda.Core;

public class BasicViewModel : ObservableObject
{
    public Connection Conn { get; private set; }
    
    public BasicViewModel(Connection conn)
    {
        this.Conn = conn;
    }

    public void Detach()
    {
        //this._driver = null;
    }
}