using System;
using System.Collections.Generic;
using System.Linq;
using Agenda.App.ControlBuilders;
using Agenda.App.Controls.Buttons;
using Agenda.App.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using ursa = Ursa.Controls;

namespace Agenda.App.Views;

public class ServerView : BaseView
{
    private Server? _server;

    private TextBox _uiTextBox_Name;
    private ComboBox _uiComboBox_Type;
    private UniformGrid _uiUniformGrid;
    private ServerFormControlBuilder _uiBuilder_ServerForm;
    private CButton _uiCButton_Save;
    private StackPanel _uiStackPanel_Main;
    
    public override void SetupUi()
    {
        Padding = new Thickness(16);
        
        _uiTextBox_Name = new TextBox()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Watermark = "Name"
        };
        _uiComboBox_Type = new ComboBox()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            IsEnabled = _server == null,
        };
        _uiUniformGrid = new UniformGrid()
        {
            //Background = new SolidColorBrush(Colors.Red),
            Columns = 2,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Top,
            ColumnSpacing = 8,
            RowSpacing = 8
        };
        _uiCButton_Save = new CButton("Save", onClick: OnClick_uiCButton_Save);
        _uiStackPanel_Main = new StackPanel() { };

        foreach (var driver in this.Manager.Drivers)
        {
            _uiComboBox_Type.Items.Add(new ComboBoxItem() {Content = driver.Value.Name, Tag = driver.Key}); //driver.Name
        }

        //_uiComboBox_Type.SelectedValue = this.Manager.Drivers.Values.ToList()[0].GetField("Name").GetValue(null);
        _uiComboBox_Type.SelectionChanged += (sender, args) => OnSelectionChanged_uiComboBox_Type();
        
        _uiStackPanel_Main.Children.Add(_uiUniformGrid);
        _uiStackPanel_Main.Children.Add(_uiCButton_Save);
        
        _uiUniformGrid.Children.Add(_uiTextBox_Name);
        _uiUniformGrid.Children.Add(_uiComboBox_Type);
        
        //Background = new SolidColorBrush(Colors.Aqua);
        Content = _uiStackPanel_Main;

        //Content = _server == null ? new ServerFormControlBuilder();
    }

    public ServerView(Manager manager, PresenterView presenterView, object? server) : base(manager, presenterView)
    {
        _server = server == null ? null : (Server)server;
    }

    private void OnSelectionChanged_uiComboBox_Type()
    {
        BuildDriverForm();
    }

    private void OnClick_uiCButton_Save(RoutedEventArgs args)
    {
        if (_uiBuilder_ServerForm == null) return;
        foreach (var field in _uiBuilder_ServerForm.GetValues())
        {
            Console.WriteLine($"Field = {field.Key}, Value = {field.Value}");
        }
    }

    private void BuildDriverForm()
    {
        if (_uiComboBox_Type.SelectedItem is ComboBoxItem item)
        {
            _uiUniformGrid.Children.RemoveRange(2, _uiUniformGrid.Children.Count - 2);
            _uiBuilder_ServerForm = new ServerFormControlBuilder(this.Manager.Drivers[item.Tag.ToString()]);
            foreach (var field in _uiBuilder_ServerForm.GetControls())
            {
                _uiUniformGrid.Children.Add(field.GetControl());
            }
        }
        //
    }
}