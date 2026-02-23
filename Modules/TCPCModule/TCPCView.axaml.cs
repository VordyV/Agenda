using System;
using System.Linq;
using System.Text;
using Agenda.Core;
using Agenda.Modules.TCPCModule.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Agenda.Modules.TCPCModule;

public partial class TcpcView : BasicView
{
    public TcpcView()
    {
        InitializeComponent();
    }
    
    public TcpcView(Connection c) : base(c)
    {
        InitializeComponent();
        this.Conn.OnStart += this.OnStart;
        this.Conn.OnStop += this.OnStop;
    }

    private void OnStart()
    {
        if (this.Conn.Driver is TCPCDriver driver) driver.OnRecvData += this.OnResponse;
        //Avalonia.Threading.Dispatcher.UIThread.Post(() => { });
        Avalonia.Threading.Dispatcher.UIThread.Post(() => this.TextBoxOutput.Items.Add(new ItemControl(DateTime.Now.ToString("HH:m:s"), "Ghost LightBlue", "event", "Connection established")));
    }
    
    private void OnStop()
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() => this.TextBoxOutput.Items.Add(new ItemControl(DateTime.Now.ToString("HH:m:s"), "Ghost LightBlue", "event", "Connection terminated")));
    }

    private async void ButtonSend_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.Conn.Driver is null || this.TextBoxInput.Text is null) return;
        var text = this.TextBoxInput.Text.Trim();
        this.TextBoxOutput.Items.Add(new ItemControl(DateTime.Now.ToString("HH:m:s"), "Red", "request", text));
        this.TextBoxInput.Text = null;
        if (this.Conn.Driver is TCPCDriver driver) await driver.SendData(Encoding.UTF8.GetBytes(text));
    }
    
    private void OnResponse(Response response)
    {
        var result = Encoding.UTF8.GetString(response.Data, 0, response.Bytes);
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            var item = new ItemControl(DateTime.Now.ToString("HH:m:s"), "Green", "response", result);
            ToolTip.SetTip(item, $"Delay: {response.Delay.ToString()}ms");
            this.TextBoxOutput.Items.Add(item);
            
            if (this.TextBoxOutput.Items.Count > 255) this.TextBoxOutput.Items.RemoveAt(0);
            
            this.TextBoxOutput.ScrollIntoView(item);
        });
    }
}