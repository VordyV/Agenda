using Agenda.Controls;
using Agenda.Core;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Agenda.Views;

public partial class DevModeView : UserControl
{
    private AgendaCore _agendaCore;
    private ViewPresenter _presenter;
    
    public DevModeView(AgendaCore agendaCore, ViewPresenter presenter)
    {
        this._agendaCore = agendaCore;
        this._presenter = presenter;
        InitializeComponent();
    }

    public DevModeView()
    {
        InitializeComponent();
    }
}