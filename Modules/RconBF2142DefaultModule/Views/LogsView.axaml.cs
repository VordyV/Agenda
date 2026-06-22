using System.Collections.ObjectModel;
using Agenda.Controls;
using Agenda.Core;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Agenda.Modules.RconBF2142DefaultModule.Views;

public class Item
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string Command { get; set; }
    public string Result { get; set; }
    public string Status { get; set; }
}

public partial class LogsView : UserControl
{
    private ViewPresenter _viewPresenter;
    private Connection _conn;
    public ObservableCollection<Item> Items { get; set; } = new();
    
    public LogsView()
    {
        InitializeComponent();
    }

    public LogsView(ViewPresenter presenter, Connection conn)
    {
        this._viewPresenter = presenter;
        this._conn = conn;
        InitializeComponent();
        this.DataContext = this;
        
        this.Items.Add(new Item() {ID = "1", Name = "console", Command = "game.sayAll \"Hi\"", Status = "?", Result = ""});
    }
}