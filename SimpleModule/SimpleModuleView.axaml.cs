using Agenda.Core;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SimpleModule;

public partial class SimpleModuleView : BasicView
{
    public SimpleModuleView()
    {
        InitializeComponent();
    }
    
    public SimpleModuleView(Connection c) : base(c)
    {
        InitializeComponent();
    }
}