using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using ursa = Ursa.Controls;

namespace Agenda.App.Views;

public class ServersView : BaseView
{
    private TextBlock _uiTextBlock;
    
    public override Control LoadUi()
    {
        _uiTextBlock = new TextBlock();
        _uiTextBlock.Text = "11";
        
        return _uiTextBlock;
    }
    
    public ServersView(Manager manager) : base(manager) { }
}