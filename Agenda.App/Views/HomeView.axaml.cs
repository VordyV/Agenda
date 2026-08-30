using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading.Tasks;
using Agenda.Controls;
using Agenda.Core;
using Agenda.Forms;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using BLite.Bson;
using Irihi.Avalonia.Shared.Helpers;
using Ursa.Controls;
using Ursa.Controls.Options;
using Module = Agenda.Core.Module;

namespace Agenda.Views;

public partial class HomeView : UserControl
{
    
    private AgendaCore _agendaCore;
    private ViewPresenter _presenter;
    
    public HomeView(AgendaCore agendaCore, ViewPresenter presenter)
    {
        this._agendaCore = agendaCore;
        this._presenter = presenter;
        InitializeComponent();
        this.DataContext = this;
        this.Loaded += async (sender, args) => { if (this._agendaCore.IsReady) await this.LoadProfileList(); };
        this._agendaCore.OnReady += async (a, b) => { await this.LoadProfileList(); };
    }

    public HomeView()
    {
        InitializeComponent();
    }

    private void ButtonNewSession_OnClick(object? sender, RoutedEventArgs e)
    {
        DialogManager.ShowOverlay(form: ctx => new ConnectForm(this._agendaCore) {DataContext = ctx});
    }

    private void AddNewProfile_OnClick(object? sender, RoutedEventArgs e)
    {
        DialogManager.ShowOverlay(form: ctx => new ProfileForm(this._agendaCore) {DataContext = ctx});
    }

    public async Task LoadProfileList()
    {
        this.WrapPanelProfiles.Children.Clear();
        Profile item;
        Module module;
        PreviewField[] fields;
        await foreach (var profile in this._agendaCore.GetAllProfiles())
        {
            module = this._agendaCore.GetModule(profile.ModuleId);
            item = new Profile() {Title = profile.Name, ObjectId = profile.Id, Subtitle = module.GetSubtitle(profile.Fields), ModuleTitle = module.Title};
            fields = this.GenPreviewFields(module.NumberPreviewFields);
            item.Fields = fields;
            item.ClickDelete += this.RemoveProfile_OnClick;
            item.ClickEdit += this.EditProfile_OnClick;
            item.ClickConnect += this.ConnectProfile_OnClick;
            item.Loaded += this.LoadedProfile_OnClick;
            this.WrapPanelProfiles.Children.Add(item);
        }
    }

    public PreviewField[] GenPreviewFields(byte number)
    {
        List<PreviewField> previewFields = new();
        for (byte i = 0; i < number; i++)
        {
            previewFields.Add(new TextPreviewField() {Label = "**********", Text = "&&&&&&&&"});
        }
        return previewFields.ToArray();
    }
    
    private async void ConnectProfile_OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Profile profile)
        {
            ProfileModel? profileModel = await this._agendaCore.GetProfile(profile.ObjectId);
            string connId = this._agendaCore.CreateNewConnection(profileModel.ModuleId, fields: profileModel.Fields);
            await this._agendaCore.InitConnection(connId);
        }
    }
    
    private void LoadedProfile_OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Profile profile) this.UpdatePreviewData(profile);
    }
    
    public void RemoveProfile(Profile profile) => this.WrapPanelProfiles.Children.Remove(profile);
    
    public async Task UpdateProfileData(Profile profile)
    {
        ProfileModel? profileModel = await this._agendaCore.GetProfile(profile.ObjectId);
        if (!this.WrapPanelProfiles.Children.Contains(profile) || profileModel == null) return;
        int index = this.WrapPanelProfiles.Children.IndexOf(profile);
        Module module = this._agendaCore.GetModule(profileModel.ModuleId);
        
        profile.Title = profileModel.Name;
        profile.Subtitle = module.GetSubtitle(profileModel.Fields);

        this.WrapPanelProfiles.Children[index] = profile;
    }

    public async Task UpdatePreviewData(Profile profile)
    {
        if (this._agendaCore.IsActiveRequestPreviewData(profile.ObjectId)) return;
        if (!this.WrapPanelProfiles.Children.Contains(profile)) return;

        try
        {
            this.ShowPreviewLoading(profile);
            Preview preview = await this._agendaCore.RequestPreviewData(profile.ObjectId, TimeSpan.FromMilliseconds(3000));
            profile.SetFields(preview.Fields);
            profile.Color = new SolidColorBrush(Color.Parse(preview.Color));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            this.HidePreviewLoading(profile);
        }
    }

    public void ShowPreviewLoading(Profile profile)
    {
        profile.ShowLoading();
    }
    
    public void HidePreviewLoading(Profile profile)
    {
        profile.HideLoading();
    }
    
    public void UpdatePreviewDataAll()
    {
        foreach (var item in this.WrapPanelProfiles.Children)
        {
            if (item is Profile profile) this.UpdatePreviewData(profile);
        }
    }
    
    private async void RemoveProfile_OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Profile profile)
        {
            await this._agendaCore.DeleteProfile(profile.ObjectId);
            this.RemoveProfile(profile);
        }
    }
    
    private async void EditProfile_OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Profile profile)
        {
            ProfileModel? profileModel = await this._agendaCore.GetProfile(profile.ObjectId);
            await DialogManager.ShowOverlayModal(form: ctx => new ProfileForm(this._agendaCore, profileModel) {DataContext = ctx});
            await this.UpdateProfileData(profile);
        }
    }

    private void IconButtonRefresh_OnClick(object? sender, RoutedEventArgs e)
    {
        this.UpdatePreviewDataAll();
    }
}