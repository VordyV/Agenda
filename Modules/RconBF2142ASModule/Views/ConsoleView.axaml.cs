using System;
using Agenda.Controls;
using Agenda.Core;
using Agenda.Modules.RconBF2142AS;
using Agenda.Modules.RconBF2142DefaultModule;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Agenda.Modules.RconBF2142ASModule.Views;

public partial class ConsoleView : BasicView
{
    private const short MaxLengthConsole = 4096;
    private ScrollViewer? _scrollTextBoxOutput;
    private bool _isRconMode = true;
    
    public ConsoleView()
    {
        InitializeComponent();
    }
    
    public ConsoleView(Connection conn) : base(conn)
    {
        InitializeComponent();
        this.Conn.OnStart += this.OnStart;
        this.Conn.OnStop += this.OnStop;
    }
    
    private void _switchButtonRcn()
    {
        this._isRconMode = !this._isRconMode;
        this.ButtonRcn.Content = this._isRconMode ? "RCN>" : "EXC>";
        
        if (this._isRconMode)
        {
            ToolTip.SetTip(this.ButtonRcn, new Random().NextDouble() < 0.7 ? "Abbreviation for RCON" : "It's not RosKomNadzor, it's RCON");
            //this.TextBoxInput.ItemsSource = RconBf2142DefaultSettings.RconCommands;
        }
        else
        {
            ToolTip.SetTip(this.ButtonRcn, "Commands of the game itself, not of the rcon module");
            //this.TextBoxInput.ItemsSource = RconBf2142DefaultSettings.ExecCommands;
        }
    }

    private void OnStart()
    {
        if (this.Conn.Driver is RconBF2142ASDriver driver) driver.TaskManager.OnChangeStatus += this.OnRecv;
    }
    
    private void OnStop()
    {
        if (this.Conn.Driver is RconBF2142ASDriver driver) driver.TaskManager.OnChangeStatus -= this.OnRecv;
    }

    private async void OnRecv(Operation oper)
    {
        if (oper.Sender == "console" && oper.Status == OperationStatus.Success)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                string result = this.TextBoxOutput.Text + $"{oper.Result}";

                if (result.Length > MaxLengthConsole)
                {
                    result = result.Substring(result.Length - MaxLengthConsole);
                }

                this.TextBoxOutput.Text = result;
                this._scrollTextBoxOutput?.ScrollToEnd();
            });
        }
    }

    private void ButtonRcn_OnClick(object? sender, RoutedEventArgs e)
    {
        this._switchButtonRcn();
    }

    private void TextBoxInput_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter || this.TextBoxInput.Text == null) return;
        var cmd = this.TextBoxInput.Text.Trim();
        if (!this._isRconMode) cmd = "exec "+cmd;
        if (this.Conn.Driver is RconBF2142ASDriver driver)
        {
            driver.TaskManager.Enqueue("console", async () => await driver.RconClient.Invoke(cmd), description: $"Executing command `{cmd}`");
        }
        this.TextBoxInput.Text = null;
    }
}