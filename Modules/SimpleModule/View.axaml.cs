using Agenda.Core;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Agenda.Modules.SimpleModule;

public partial class View : UserControl
{
    private BasicDriver _driver;
    
    public View()
    {
        InitializeComponent();
    }
    
    public View(BasicDriver driver)
    {
        this._driver = driver;
        InitializeComponent();
    }
}