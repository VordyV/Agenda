using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Agenda.Controls;
using Agenda.Core;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;

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
    
    public PlayersView()
    {
        InitializeComponent();
    }
    
    public PlayersView(ViewPresenter presenter, Connection conn)
    {
        this._viewPresenter = presenter;
        this._conn = conn;
        InitializeComponent();
        this._timer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromSeconds(5)
        }; 
        this._timer.Tick += OnTimerTick;
        
        this._conn.OnStart += this.OnStart;
        this._conn.OnStop += this.OnStop;
        
        if (this._conn.IsStarted) this.OnStart();
        
        this.DataGridPlayers.Columns.Add(new DataGridTextColumn() {FontWeight = FontWeight.Bold, Header = "Nick", Binding = new Binding("Nick"), IsReadOnly = true});
        this.DataGridPlayers.Columns.Add(new DataGridTextColumn() {Header = "PID", Binding = new Binding("PID"), IsReadOnly = true});
        this.DataGridPlayers.Columns.Add(new DataGridTextColumn() {Header = "Address", Binding = new Binding("Address"), IsReadOnly = true});
        this.DataGridPlayers.Columns.Add(new DataGridTextColumn() {Header = "Hash", Binding = new Binding("Hash"), IsReadOnly = true});
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
        if (rt.Name == "players")
        {
            if (rt.Result is null) return;
            Dispatcher.UIThread.Post(() =>
            {
                this.DataGridPlayers.ItemsSource = GetPlayerDetails(rt.Result);
            });
        }
    }
    
    public static List<Player> GetPlayerDetails(string rawData)
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
}