using System;
using System.Linq;
using Agenda.Controls;
using Agenda.Core;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;

namespace Agenda.Modules.RconBF2142DefaultModule.Views;

public partial class ConsoleView : UserControl
{
    private ViewPresenter _viewPresenter;
    private Connection _conn;
    private const short MaxLengthConsole = 4096;
    private ScrollViewer? _scrollTextBoxOutput;
    private bool _isRconMode = true;
    
    public ConsoleView()
    {
        InitializeComponent();
    }

    public ConsoleView(ViewPresenter presenter, Connection conn)
    {
        this._viewPresenter = presenter;
        this._conn = conn;
        InitializeComponent();
        
        this._conn.OnStart += this.OnStart;
        this._conn.OnStop += this.OnStop;
        
        if (this._conn.IsStarted) this.OnStart();
        
        this._switchButtonRcn();
        
        this._scrollTextBoxOutput = this.TextBoxOutput.GetVisualDescendants().OfType<ScrollViewer>().FirstOrDefault();
    }
    
    private void OnStart()
    {
        Console.WriteLine("start");
        if (this._conn.Driver is RconBf2142DefaultDriver driver) driver.OnRecv += this.OnRecv;
        //Avalonia.Threading.Dispatcher.UIThread.Post(() => { });
        //Avalonia.Threading.Dispatcher.UIThread.Post(() => this.TextBoxOutput.Items.Add(new ItemControl(DateTime.Now.ToString("HH:m:s"), "Ghost LightBlue", "event", "Connection established")));
    }
    
    private void OnStop()
    {
        Console.WriteLine("stop");
        if (this._conn.Driver is RconBf2142DefaultDriver driver) driver.OnRecv -= this.OnRecv;
    }

    private async void OnRecv(RconTask rt)
    {
        //Console.WriteLine($"name = {rt.Name}, cmd = {rt.Command}, result = {rt.Result}");
        if (rt.Name == "console")
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                string result = this.TextBoxOutput.Text + $"{rt.Result}\n";

                if (result.Length > MaxLengthConsole)
                {
                    result = result.Substring(result.Length - MaxLengthConsole);
                }

                this.TextBoxOutput.Text = result;
                this._scrollTextBoxOutput?.ScrollToEnd();
            });
        }
        
    }

    private async void TextBoxInput_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter || this.TextBoxInput.Text == null) return;
        var cmd = this.TextBoxInput.Text.Trim();
        if (!this._isRconMode) cmd = "exec "+cmd;
        if (this._conn.Driver is RconBf2142DefaultDriver driver) await driver.SendAsync("console", cmd);
        this.TextBoxInput.Text = null;
    }

    private void _switchButtonRcn()
    {
        this._isRconMode = !this._isRconMode;
        this.ButtonRcn.Content = this._isRconMode ? "RCN>" : "EXC>";
        
        if (this._isRconMode)
        {
            ToolTip.SetTip(this.ButtonRcn, new Random().NextDouble() < 0.7 ? "Abbreviation for RCON" : "It's not RosKomNadzor, it's RCON");
            this.TextBoxInput.ItemsSource = RconBf2142DefaultSettings.RconCommands;
        }
        else
        {
            ToolTip.SetTip(this.ButtonRcn, "Commands of the game itself, not of the rcon module");
            this.TextBoxInput.ItemsSource = RconBf2142DefaultSettings.ExecCommands;
        }
    }

    private void TextBlockRcn_OnClick(object? sender, RoutedEventArgs e)
    {
        this._switchButtonRcn();
    }
}