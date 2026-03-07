using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Agenda.Controls;
using Agenda.Core;
using Agenda.Forms;
using Agenda.Modules.RconBF2142DefaultModule.Forms;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Ursa.Controls;

namespace Agenda.Modules.RconBF2142DefaultModule.Views;

public class Player
{
    public string Nick { get; set; }
    public string PID { get; set; }
    public string Address { get; set; }
    public string Port { get; set; }
    public string Hash { get; set; }
}

public partial class PlayersView : UserControl
{
    private ViewPresenter _viewPresenter;
    private Connection _conn;
    private DispatcherTimer _timer;
    public ObservableCollection<Player> Players { get; set; } = new();
    
    public PlayersView()
    {
        InitializeComponent();
    }
    
    public PlayersView(ViewPresenter presenter, Connection conn)
    {
        this._viewPresenter = presenter;
        this._conn = conn;
        InitializeComponent();
        DataContext = this;
        
        this._timer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromSeconds(3)
        }; 
        this._timer.Tick += OnTimerTick;
        
        this._conn.OnStart += this.OnStart;
        this._conn.OnStop += this.OnStop;
        
        if (this._conn.IsStarted) this.OnStart();
        
        this.Players.Add(new Player() {Nick = "2"});
        this.Players.Add(new Player() {Nick = "3"});
        
        //this.DataGridPlayers.Columns.Add(new DataGridTextColumn() {FontWeight = FontWeight.Bold, Header = "Nick", Binding = new Binding("Nick"), IsReadOnly = true});
        //this.DataGridPlayers.Columns.Add(new DataGridTextColumn() {Header = "PID", Binding = new Binding("PID"), IsReadOnly = true});
        //this.DataGridPlayers.Columns.Add(new DataGridTextColumn() {Header = "Address", Binding = new Binding("Address"), IsReadOnly = true});
        //this.DataGridPlayers.Columns.Add(new DataGridTextColumn() {Header = "Hash", Binding = new Binding("Hash"), IsReadOnly = true});
    }
    
    private async void OnStart()
    {
        this._timer.Start();

        if (this._conn.Driver is RconBf2142DefaultDriver driver)
        {
            driver.OnRecv += this.OnRecv;
            await driver.SendAsync("players", "exec admin.listplayers");
        }
        //Avalonia.Threading.Dispatcher.UIThread.Post(() => { });
    }
    
    private void OnStop()
    {
        this._timer.Stop();
        if (this._conn.Driver is RconBf2142DefaultDriver driver) driver.OnRecv -= this.OnRecv;
    }

    private async void OnTimerTick(object? sender, EventArgs e)
    {
        if (this._conn.Driver is RconBf2142DefaultDriver driver) await driver.SendAsync("players", "exec admin.listplayers");
    }

    private async void OnRecv(RconTask rt)
    {
        Console.WriteLine(rt.Name);
        if (rt.Name == "players")
        {
            if (rt.Result is null) return;
            
            this.Players.Clear();
            foreach (var player in this._getPlayerDetails(rt.Result))
            {
                this.Players.Add(player);
            }
            
            //Dispatcher.UIThread.Post(() =>
            //{
            //    
            //});
        }
        else if (rt.Name == "players_kick")
        {
            if (rt.Result == "") NotificationManager.ShowInfo("Operation completed", $"Player {rt.Details["nick"]}({rt.Details["pid"]}) kicked");
            else NotificationManager.ShowError("Operation failed", $"Player {rt.Details["nick"]}({rt.Details["pid"]}) not kicked: {rt.Result}");
        }
    }
    
    private List<Player> _getPlayerDetails(string rawData)
    {
        var players = new List<Player>();
        
        string pattern = @"^Id:\ +(\d+)                  # PlayerID
                          \ -\ (.*?)                      # Player Name
                          \ is\ remote\ ip:\ (\d+\.\d+\.\d+\.\d+):  # IP Address
                          (\d+)                           # Port Number
                          (?:.*?hash:\ (\w{32}))?         # CD Key Hash";
        
        var regex = new Regex(pattern, 
            RegexOptions.Multiline | 
            RegexOptions.IgnorePatternWhitespace | 
            RegexOptions.Singleline);
        
        foreach (Match match in regex.Matches(rawData))
        {
            if (match.Success && match.Groups.Count >= 6)
            {
                var player = new Player()
                {
                    PID = match.Groups[1].Value,
                    Nick = match.Groups[2].Value.Trim(),
                    Address = match.Groups[3].Value,
                    Port = match.Groups[4].Value,
                    Hash = match.Groups[5].Success ? match.Groups[5].Value : "???"
                };
                
                players.Add(player);
            }
        }
        return players;
    }

    private async void MenuItemDGKick_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataGridPlayers.SelectedItem is Player player)
        {
            var context = new DialogContext();
            string? result = await OverlayDialog.ShowCustomModal<string>(new KickForm() {DataContext = context}, context, hostId: "main");
            if (result is null) return;
            
            if (this._conn.Driver is RconBf2142DefaultDriver driver) await driver.SendAsync("players_kick", $"exec admin.kickPlayer {player.PID}", new() {{"nick", player.Nick}, {"pid", player.PID}});
        }
    }
    
    private async void MenuItemDGBan_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataGridPlayers.SelectedItem is Player player)
        {
            Console.WriteLine(player.Nick);
        }
    }
}