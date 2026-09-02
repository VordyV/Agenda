using System;
using System.Collections.Generic;
using Agenda.Core;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Irihi.Avalonia.Shared.Contracts;
using Ursa.Controls;
using Agenda.Core.ModelFieldControls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Styling;
using BLite.Bson;

namespace Agenda.Forms;

public partial class ProfileForm : Form
{
    private AgendaCore _agendaCore;
    private Module _currentModule;
    private ProfileModel? _profileModel;
    private UserControl? _homeView;
    private Dictionary<string, Control> _fields = new();
    
    public ProfileForm(AgendaCore agendaCore, ProfileModel? profileModel = null, UserControl? homeView = null)
    {
        this._agendaCore = agendaCore;
        this._profileModel = profileModel;
        this._homeView = homeView;
        
        InitializeComponent();
        
        this.TextBlockTitleNew.IsVisible = profileModel == null ? true : false;
        this.TextBlockTitleUpdate.IsVisible = profileModel != null ? true : false;
        this.ComboBoxType.IsEnabled = profileModel == null ? true : false;
        this.TextBoxName.Text = profileModel == null ? "" : profileModel.Name;
        this.ButtonAddProfile.IsVisible = profileModel == null ? true : false;
        this.ButtonUpdateProfile.IsVisible = profileModel != null ? true : false;
        
        this.Loaded += (sender, args) => this.LoadModules();
    }
    
    public ProfileForm()
    {
        InitializeComponent();
    }
    
    private void SetTags(Tag[]? tags)
    {
        if (tags == null) return;
        Label label;
        this.WrapPanelTags.Children.Clear();
        foreach (var tag in tags)
        {
            label = new Label() {Content = tag.Text, Classes = { "Ghost", tag.Color }, Theme = this.FindResource("TagLabel") as ControlTheme};
            this.WrapPanelTags.Children.Add(label);
        }
    }
    
    private void LoadModules()
    {
        var modules = this._agendaCore.GetModules();
        ushort i = 0;
        ushort index = 0;
        
        foreach (var module in modules)
        {
            if (this._profileModel != null && module.Id == this._profileModel.ModuleId) index = i;
            this.ComboBoxType.Items.Add(new ComboBoxItem() {Content = module.Title, Name = module.Id});
            i++;
        }
        this.ComboBoxType.SelectedIndex = this._profileModel != null ? index : 0;

        this._currentModule = modules[this._profileModel != null ? index : 0];
        this.SetTags(this._currentModule.Tags);
    }

    private void ComboBoxType_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var item = (ComboBoxItem)this.ComboBoxType.SelectedItem;
        this._currentModule = this._agendaCore.GetModule(item.Name);
        this.SetTags(this._currentModule.Tags);

        this.TextBlockDescription.Text = this._currentModule.Description;
        
        this.FormDynamic.Items.Clear();
        this._fields.Clear();
        
        //var i = this._profileModel.Fields;
        foreach (var field in this._currentModule.Fields)
        {
            
            BaseModelFieldControl control = field.Value.Control.Invoke();
            control.Width = 400;
            if (field.Value.Value is not null && this._profileModel == null) control.SetValue(field.Value.Value);
            else if (this._profileModel != null) control.SetValue(this._profileModel?.Fields[field.Key]);
            var formItem = new FormItem() { Content = control, Name = field.Key};
            FormItem.SetLabel(formItem, field.Value.Title);
            FormItem.SetIsRequired(formItem, field.Value.Required);
            
            this.FormDynamic.Items.Add(formItem);
            this._fields.Add(field.Key, control.GetControl());
        }
    }

    private async void ButtonAddProfile_OnClick(object? sender, RoutedEventArgs e)
    {
        DataValidationErrors.ClearErrors(this.TextBoxName);
        if (this.TextBoxName.Text == null || this.TextBoxName.Text.Trim() == "")
        {
            DataValidationErrors.SetError(this.TextBoxName, new DataValidationException("Name must not be empty"));
            return;
        }
        
        bool isRun = true;

        Dictionary<string, string?> fields = new();
        
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

        try
        {
            if (this._profileModel == null) await this._agendaCore.AddProfile(name: this.TextBoxName.Text, moduleId: this._currentModule.Id, fields: fields);
            else
            {
                this._profileModel.Name = this.TextBoxName.Text;
                this._profileModel.Fields = fields;
                await this._agendaCore.UpdateProfile(this._profileModel);
            }
        }
        catch (Exception exception)
        {
            DataValidationErrors.SetError(this.TextBoxName, new DataValidationException(exception.Message));
            return;
        }
        
        if (DataContext is IDialogContext ctx)  ctx.Close();
    }
}