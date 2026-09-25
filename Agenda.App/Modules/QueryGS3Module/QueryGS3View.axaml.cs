using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Agenda.Core;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using AvaloniaHex.Document;
using Ursa.Controls;
using Avalonia.Threading;

namespace Agenda.Modules.QueryGS3Module;

public class Player
{
    public string Id { get; set; }
    public string PID { get; set; }
    public string Nick { get; set; }
    public string Scope { get; set; }
    public string Skill { get; set; }
    public string Deaths { get; set; }
    public string Team { get; set; }
    public string Ping { get; set; }
}

public partial class QueryGS3View : BasicView
{
    public ObservableCollection<Player> Players { get; set; } = new();

    public QueryGS3View(Connection c) : base(c)
    {
        InitializeComponent();
        this.DataContext = this;
        this.Loaded += this._loaded;
    }

    public QueryGS3View()
    {
        InitializeComponent();
    }

    private async Task _ping()
    {
        if (this.Conn.Driver is QueryGS3Driver driver)
        {
            string ping = (await driver.GetPint()).ToString();
            
            Dispatcher.UIThread.Post(() =>
            {
                this.TextBlockPing.Text = ping;
            });
        }
    }

    private void _setStatus(string color, string text)
    {
        this.LabelStatus.Classes.Clear();
        this.LabelStatus.Classes.Add("Solid");
        this.LabelStatus.Classes.Add(color);
        this.LabelStatus.Content = text;
    }

    private async void _loaded(object? sender, RoutedEventArgs args)
    {
        _ = this._ping();
        if (this.Conn.Driver is QueryGS3Driver driver)
        {
            this._setStatus("Blue", "Update");
            ServerStatus? serverStatus = await driver.GetData();
            
            if (serverStatus is not null && serverStatus.IsParsedSuccessfully) //online
            {
                this.TextBlockDelay.Text = $"{serverStatus.Delay.ToString()} ms";

                this._setStatus("Green", "Online");
                
                this._updateParams(serverStatus);
                this._updateRawData(serverStatus.RawData);
                
                this.Players.Clear();
                int i = 0;
                foreach (var player in serverStatus.Players)
                {
                    i++;
                    this.Players.Add(new Player() {Id = i.ToString(), PID = player.ContainsKey("pid") ? player["pid"] : "missing", Deaths = player.ContainsKey("deaths") ? player["deaths"] : "missing", Skill = player.ContainsKey("skill") ? player["skill"] : "missing", Nick = player.ContainsKey("name") ? player["name"] : "missing", Ping = player.ContainsKey("ping") ? player["ping"] : "missing", Scope = player.ContainsKey("scope") ? player["scope"] : "missing", Team = player.ContainsKey("team") ? player["team"] : "missing"});
                }
                
                if (serverStatus.Info.TryGetValue("hostname", out string hostname)) { this.TextBlockServerName.Text = hostname; }
                else { this.TextBlockServerName.Text = "???"; }
                
                if (serverStatus.Info.TryGetValue("mapname", out string mapname)) { this.TextBlockMap.Text = mapname.Replace("_", " "); }
                else { this.TextBlockMap.Text = "???"; }
                
                if (serverStatus.Info.TryGetValue("gametype", out string gametype)) { this.TextBlockMod.Text = gametype.Replace("_", " "); }
                else { this.TextBlockMod.Text = "???"; }
                
                if (serverStatus.Info.TryGetValue("numplayers", out string numplayers) && serverStatus.Info.TryGetValue("maxplayers", out string maxplayers)) { this.TextBlockServerPlayers.Text = $"{numplayers} / {maxplayers}"; }
                else { this.TextBlockServerPlayers.Text = "- / -"; }
            }
            else //offline
            {
                this.TextBlockDelay.Text = $"-1 ms";
                
                this._setStatus("Red", "Offline");
                
                this.TextBlockServerName.Text = $"{this.Conn.Fields["address"]}:{this.Conn.Fields["query_port"]}";
                this.TextBlockMap.Text = "???";
                this.TextBlockMod.Text = "???";
                this.TextBlockServerPlayers.Text = "- / -";
            }
        }
    }

    private void _updateParams(ServerStatus serverStatus)
    {
        this.DescriptionsParams.Items.Clear();
        foreach (var param in serverStatus.Info)
        {
            this.DescriptionsParams.Items.Add(new DescriptionsItem() {Label = param.Key, Content = string.IsNullOrEmpty(param.Value) ? new Label() {Content = "missing", Classes = { "Gray" }, Theme = this.FindResource("TagLabel") as ControlTheme} : new SelectableTextBlock() {Text = param.Value}});
        }
    }

    private void _updateRawData(byte[] rawData)
    {
        this.HexEditorRawData.Document = new MemoryBinaryDocument(rawData);
    }

    private async void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.Conn.Driver is QueryGS3Driver driver)
        {
            await driver.GetData();
        }
    }
}