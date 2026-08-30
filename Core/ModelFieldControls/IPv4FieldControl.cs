using System;
using System.Net;
using Avalonia.Controls;
using Ursa.Controls;

namespace Agenda.Core.ModelFieldControls;

public class IPv4FieldControl : BaseModelFieldControl
{
    private Ursa.Controls.IPv4Box _ipv4Box;
    
    public IPv4FieldControl()
    {
        _ipv4Box = new Ursa.Controls.IPv4Box() {InputMode = IPv4BoxInputMode.Fast};
        this.Content = _ipv4Box;
    }
    
    public override string? GetValue()
    {
        return this._ipv4Box.IPAddress?.ToString();
    }

    public override void SetValue(string? value) => this._ipv4Box.IPAddress = IPAddress.Parse(value);
    
    public override Control GetControl() => this._ipv4Box;
}