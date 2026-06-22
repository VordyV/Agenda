using System;
using System.Collections.ObjectModel;
using Agenda.Controls;
using Agenda.Core;
using Agenda.Forms;
using Agenda.Modules.RconBF2142AS;
using Agenda.Modules.RconBF2142ASModule.Forms;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using rconnet.RconBf2142.Default;
using Ursa.Controls;
using Ban = Agenda.Modules.RconBF2142DefaultModule.Ban;

namespace Agenda.Modules.RconBF2142ASModule.Views;

public class Player
{
    public string Nick { get; set; }
    public string PID { get; set; }
    public string Address { get; set; }
    public string Port { get; set; }
    public string Hash { get; set; }
}

public partial class PlayersView : BasicView
{
    public ObservableCollection<Player> Players { get; set; } = new();
    
    private DispatcherTimer _timer;
    
    public PlayersView()
    {
        InitializeComponent();
    }
    
    public PlayersView(Connection conn) : base(conn)
    {
        InitializeComponent();
        this.DataContext = this;
        
        this._timer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromSeconds(3)
        }; 
        this._timer.Tick += OnTimerTick;
        
        this.Conn.OnStart += this.OnStart;
        this.Conn.OnStop += this.OnStop;
        
        if (this.Conn.IsStarted) this.OnStart();
        
        this.Players.Add(new Player() {Nick = "2"});
        this.Players.Add(new Player() {Nick = "3"});
    }
    
    private async void OnRecv(Operation oper)
    {
        //
        if (oper.Sender == "player_list" && oper.Status == OperationStatus.Success)
        {
            PlayerList playerList = (PlayerList)oper.Result;
            //if (playerList == null) return;
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                this.Players.Clear();
                foreach (var player in playerList.Items)
                {
                    this.Players.Add(new Player() {PID = player.Key.ToString(), Nick = player.Value.Nick, Address = player.Value.Address.ToString(), Hash = player.Value.Key ?? ""});
                }
            });
            //Console.WriteLine(oper.Result);
            //Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            //foreach (var player in playerList.Items)
            //{
            //    this.Players.Add(new Player() {PID = player.Key.ToString(), Nick = player.Value.Nick, Address = player.Value.Address.ToString(), Hash = player.Value.Key ?? ""});
            //}
        }
    }

    private void Update()
    {
        if (this.Conn.Driver is RconBF2142ASDriver driver)
        {
            driver.TaskManager.Enqueue("player_list", async () => await driver.RconClient.GetPlayers(), description: $"Updating player list");
        }
    }
    
    private void OnStart()
    {
        this._timer.Start();
        if (this.Conn.Driver is RconBF2142ASDriver driver) driver.TaskManager.OnChangeStatus += this.OnRecv;
        this.Update();
    }
    
    private void OnStop()
    {
        this._timer.Stop();
        if (this.Conn.Driver is RconBF2142ASDriver driver) driver.TaskManager.OnChangeStatus -= this.OnRecv;
        //if (this.Conn.Driver is RconBF2142ASDriver driver) driver.OnRecv -= this.OnRecv;
    }
    
    private async void OnTimerTick(object? sender, EventArgs e)
    {
        this.Update();
    }

    private async void MenuItemDGKick_OnClick(object? sender, RoutedEventArgs e)
    {
        
    }

    private async void MenuItemDGBan_OnClick(object? sender, RoutedEventArgs e)
    {
        
    }

    private async void ButtonBan_OnClick(object? sender, RoutedEventArgs e)
    {
        var context = new DialogContext();
        Ban? result = await OverlayDialog.ShowCustomModal<Ban>(new BanForm(nick: "TestPlayer") {DataContext = context}, context, hostId: "main");
        if (result is null) return;
    }
}