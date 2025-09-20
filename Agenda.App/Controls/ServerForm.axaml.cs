using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Agenda.App.Controls;

public class ServerForm : TemplatedControl
{
    
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<ServerForm, string>(nameof(Title), "{Title}");
    
    public static readonly StyledProperty<string> SubTitleProperty =
        AvaloniaProperty.Register<ServerForm, string>(nameof(SubTitle), "{SubTitle}");
    
    public static readonly StyledProperty<ICommand?> CommandConnectProperty =
        AvaloniaProperty.Register<ServerForm, ICommand?>(nameof(CommandConnect));
    
    public static readonly StyledProperty<ICommand?> CommandEditProperty =
        AvaloniaProperty.Register<ServerForm, ICommand?>(nameof(CommandEdit));
    
    public static readonly StyledProperty<ICommand?> CommandRemoveProperty =
        AvaloniaProperty.Register<ServerForm, ICommand?>(nameof(CommandRemove));
    
    
    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<ServerForm, object?>(nameof(CommandParameter));
    

    public ICommand? CommandConnect
    {
        get => GetValue(CommandConnectProperty);
        set => SetValue(CommandConnectProperty, value);
    }
    
    public ICommand? CommandEdit
    {
        get => GetValue(CommandEditProperty);
        set => SetValue(CommandEditProperty, value);
    }
    
    public ICommand? CommandRemove
    {
        get => GetValue(CommandRemoveProperty);
        set => SetValue(CommandRemoveProperty, value);
    }
    
    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }
    
    public event EventHandler? ClickConnect;
    public event EventHandler? ClickEdit;
    public event EventHandler? ClickRemove;

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string SubTitle
    {
        get => GetValue(SubTitleProperty);
        set => SetValue(SubTitleProperty, value);
    }
    
    protected void RaiseClickConnect()
    {
        ClickConnect?.Invoke(this, EventArgs.Empty);

        if (CommandConnect?.CanExecute(CommandParameter) == true)
            CommandConnect.Execute(CommandParameter);
    }
    
    protected void RaiseClickEdit()
    {
        ClickEdit?.Invoke(this, EventArgs.Empty);

        if (CommandEdit?.CanExecute(CommandParameter) == true)
            CommandEdit.Execute(CommandParameter);
    }
    
    protected void RaiseClickRemove()
    {
        ClickRemove?.Invoke(this, EventArgs.Empty);

        if (CommandRemove?.CanExecute(CommandParameter) == true)
            CommandRemove.Execute(CommandParameter);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        
        if (e.NameScope.Find<SplitButton>("PART_Button_Connect") is { } button1)
        {
            button1.Click += (_, _) => RaiseClickConnect();
        }
        
        if (e.NameScope.Find<MenuItem>("PART_MenuItem_Edit") is { } button2)
        {
            button2.Click += (_, _) => RaiseClickEdit();
        }
        
        if (e.NameScope.Find<MenuItem>("PART_MenuItem_Remove") is { } button3)
        {
            button3.Click += (_, _) => RaiseClickRemove();
        }
    }
}