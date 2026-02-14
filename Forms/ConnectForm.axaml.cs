using System;
using System.Collections.Generic;
using System.Linq;
using Agenda.Controls;
using Agenda.Core;
using Agenda.Core.ModelFieldControls;
using Agenda.Forms.ConnectionIndicatorForms;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using Irihi.Avalonia.Shared.Contracts;
using Ursa.Controls;

namespace Agenda.Forms;

public partial class ConnectForm : UserControl
{
    private Manager _manager;
    private ViewPresenter _presenter;
    private Module _currentModule;
    private Dictionary<string, Control> _fields = new();
    
    public ConnectForm(Manager manager, ViewPresenter presenter)
    {
        this._manager = manager;
        this._presenter = presenter;
        InitializeComponent();
        this.LoadModules();
    }

    public ConnectForm()
    {
        InitializeComponent();
    }

    private void LoadModules()
    {
        var modules = this._manager.GetModules();
        
        foreach (var module in modules)
        {
            this.ComboBoxType.Items.Add(new ComboBoxItem() {Content = module.Title, Name = module.Id});
        }
        this.ComboBoxType.SelectedIndex = 0;

        this._currentModule = modules[0];
    }

    private void ComboBoxType_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var item = (ComboBoxItem)this.ComboBoxType.SelectedItem;
        this._currentModule = this._manager.GetModule(item.Name);
        
        this.FormDynamic.Items.Clear();
        this._fields.Clear();
        
        foreach (var field in this._currentModule.Fields)
        {
            BaseModelFieldControl control = field.Value.Control.Invoke();
            control.Width = 400;
            var formItem = new FormItem()
            { Content = control, Name = field.Key};
            FormItem.SetLabel(formItem, field.Value.Title);
            FormItem.SetIsRequired(formItem, field.Value.Required);
            
            this.FormDynamic.Items.Add(formItem);
            this._fields.Add(field.Key, control.GetControl());
        }
    }

    private async void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        bool isRun = true;

        Dictionary<string, object?> fields = new();
        
        foreach (FormItem field in this.FormDynamic.Items)
        {
            var fieldName = field.Name;
            var fieldControl = (BaseModelFieldControl)field.Content;
            var value = fieldControl.GetValue();
            
            Control control = this._fields[field.Name];
            
            DataValidationErrors.ClearErrors(control);
            
            if (value == null)
            {
                isRun = false;
                DataValidationErrors.SetError(control, new DataValidationException("This is mandatory to fill out"));
                continue;
            }

            var validator = this._currentModule.Fields[fieldName].Validator;
            
            if (validator != null)
            {
                var validatorResult = validator.Invoke(value);
            
                if (!validatorResult.Success)
                {
                    isRun = false;

                    foreach (var error in validatorResult.Errors)
                    {
                        DataValidationErrors.SetError(control, new Exception(error.ToString()));
                    }
                }
            }
            
            fields.Add(field.Name, value);
        }

        if (!isRun) return;
        
        var connId = this._manager.CreateNewConnection(this._currentModule.Id, fields);
        
        //this._presenter.LoadView("server", connId);
        
        //if (DataContext is IDialogContext ctx) ctx.Close();
    }
}