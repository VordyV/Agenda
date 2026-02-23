using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Agenda.Modules.TCPCModule.Controls;

public partial class ItemControl : UserControl
{
    public ItemControl()
    {
        InitializeComponent();
    }
    
    public ItemControl(string time, string typeClass, string typeText, string data)
    {
        InitializeComponent();
        this.LabelType.Classes.Add(typeClass);
        this.LabelType.Content = typeText;
        this.LabelTime.Content = time;
        this.SelectableTextBlockData.Text = data;
    }
}