using System;
using System.Collections.ObjectModel;
using Agenda.Controls;
using Agenda.Core;
using Agenda.Modules.RconBF2142AS;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Agenda.Modules.RconBF2142ASModule.Views;

public class Item
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public OperationStatus Status { get; set; }
    public bool IsStatusCreated => Status == OperationStatus.Created;
    public bool IsStatusCancelled => Status == OperationStatus.Cancelled;
    public bool IsStatusInProgress => Status == OperationStatus.InProgress;
    public bool IsStatusError => Status == OperationStatus.Error;
    public bool IsStatusSuccess => Status == OperationStatus.Success;
}

public partial class LogsView : BasicView
{
    public ObservableCollection<Item> Items { get; set; } = new();
    
    protected ViewPresenter Presenter;
    
    public LogsView()
    {
        InitializeComponent();
    }

    public LogsView(Connection conn, ViewPresenter presenter) : base(conn)
    {
        this.Presenter = presenter;
        
        InitializeComponent();
        
        Console.WriteLine(5);
        this.DataContext = this;

        this.Presenter.OnShowView += this.OnShow;
        this.Items.Add(new Item() {ID = "1", Name = "console", Status = OperationStatus.Created});
    }

    protected void OnShow(string view)
    {
        if (view == "logs") this.Update();
    }

    protected void Update()
    {
        if (this.Conn.Driver is RconBF2142ASDriver driver)
        {
            this.Items.Clear();
            foreach (var oper in driver.TaskManager.Operations)
            {
                this.Items.Add(new Item() {ID = oper.Key, Name = oper.Value.Sender, Status = oper.Value.Status, Description = oper.Value.Description});
            }
        }
    }
}