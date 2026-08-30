using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Agenda.Core;

public abstract class BasicView : UserControl
{
    public Connection Conn { get; private set; }
    
    public BasicView(Connection conn)
    {
        this.Conn = conn;
    }

    public BasicView()
    {
        
    }
}