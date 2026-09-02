using System;
using System.Collections.Generic;
using Agenda.Core;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using BLite.Bson;
using Ursa.Controls;

namespace Agenda.Controls;

[TemplatePart("PART_DescriptionsFields", typeof(Descriptions))]
[TemplatePart("PART_IconButtonEdit", typeof(IconButton))]
[TemplatePart("PART_IconButtonDelete", typeof(IconButton))]
[TemplatePart("PART_IconButtonConnect", typeof(IconButton))]
[TemplatePart("PART_WrapPanelTags", typeof(WrapPanel))]
public class Profile : TemplatedControl
{
    private Descriptions? _descriptionsFields;
    private IconButton? _iconButtonEdit;
    private IconButton? _iconButtonDelete;
    private IconButton? _iconButtonConnect;
    private WrapPanel? _WrapPanelTags;

    public ObjectId ObjectId { get; set; }
    public PreviewField[] Fields { get; set; }
    public Tag[] Tags { get; set; }
    
    // Property Name
    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<Profile, string>(nameof(Title), defaultValue: "Name");
    public string Title { get => GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    
    // Property Subtitle
    public static readonly StyledProperty<string> SubtitleProperty = AvaloniaProperty.Register<Profile, string>(nameof(Subtitle), defaultValue: "Subtitle");
    public string Subtitle { get => GetValue(SubtitleProperty); set => SetValue(SubtitleProperty, value); }
    
    // Property ModuleTitle
    public static readonly StyledProperty<string> ModuleTitleProperty = AvaloniaProperty.Register<Profile, string>(nameof(ModuleTitle), defaultValue: "ModuleTitle");
    public string ModuleTitle { get => GetValue(ModuleTitleProperty); set => SetValue(ModuleTitleProperty, value); }
    
    // Property Color
    public static readonly StyledProperty<IBrush> ColorProperty = AvaloniaProperty.Register<Profile, IBrush>(nameof(Color));
    public IBrush Color { get => GetValue(ColorProperty); set => SetValue(ColorProperty, value); }
    
    // Property ClickEdit
    public readonly RoutedEvent<RoutedEventArgs> ClickEditEvent = RoutedEvent.Register<IconButton, RoutedEventArgs>(nameof(ClickEdit), RoutingStrategies.Bubble);
    public event EventHandler<RoutedEventArgs>? ClickEdit { add => AddHandler(ClickEditEvent, value); remove => RemoveHandler(ClickEditEvent, value); }
    
    // Property ClickDelete
    public readonly RoutedEvent<RoutedEventArgs> ClickDeleteEvent = RoutedEvent.Register<IconButton, RoutedEventArgs>(nameof(ClickDelete), RoutingStrategies.Bubble);
    public event EventHandler<RoutedEventArgs>? ClickDelete { add => AddHandler(ClickDeleteEvent, value); remove => RemoveHandler(ClickDeleteEvent, value); }
    
    // Property ClickConnect
    public readonly RoutedEvent<RoutedEventArgs> ClickConnectEvent = RoutedEvent.Register<IconButton, RoutedEventArgs>(nameof(ClickConnect), RoutingStrategies.Bubble);
    public event EventHandler<RoutedEventArgs>? ClickConnect { add => AddHandler(ClickConnectEvent, value); remove => RemoveHandler(ClickConnectEvent, value); }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        
        if (this._iconButtonEdit is not null) this._iconButtonEdit.Click -= OnClickIconButtonEdit;
        if (this._iconButtonDelete is not null) this._iconButtonDelete.Click -= OnClickIconButtonDelete;
        if (this._iconButtonConnect is not null) this._iconButtonConnect.Click -= this.OnClickIconButtonConnect;
        
        this._descriptionsFields = e.NameScope.Get<Descriptions>("PART_DescriptionsFields");
        this._iconButtonEdit = e.NameScope.Get<IconButton>("PART_IconButtonEdit");
        this._iconButtonDelete = e.NameScope.Get<IconButton>("PART_IconButtonDelete");
        this._iconButtonConnect = e.NameScope.Get<IconButton>("PART_IconButtonConnect");
        this._WrapPanelTags = e.NameScope.Get<WrapPanel>("PART_WrapPanelTags");
        
        this._iconButtonEdit.Click += OnClickIconButtonEdit;
        this._iconButtonDelete.Click += OnClickIconButtonDelete;
        this._iconButtonConnect.Click += this.OnClickIconButtonConnect;
        
        this.SetFields(Fields);
        this.SetTags(Tags);
    }

    private void OnClickIconButtonEdit(object? sender, RoutedEventArgs e) => RaiseEvent(new RoutedEventArgs(this.ClickEditEvent));
    private void OnClickIconButtonDelete(object? sender, RoutedEventArgs e) => RaiseEvent(new RoutedEventArgs(this.ClickDeleteEvent));
    private void OnClickIconButtonConnect(object? sender, RoutedEventArgs e) => RaiseEvent(new RoutedEventArgs(this.ClickConnectEvent));

    public void SetTags(Tag[]? tags)
    {
        if (this._WrapPanelTags == null || tags == null) return;
        Label label;
        foreach (var tag in tags)
        {
            label = new Label() {Content = tag.Text, Classes = { "Ghost", tag.Color }, Theme = this.FindResource("TagLabel") as ControlTheme};
            this._WrapPanelTags.Children.Add(label);
        }
    }
    
    public void SetFields(PreviewField[]? previewFields)
    {
        if (this._descriptionsFields == null || previewFields == null) return;
        this._descriptionsFields.Items.Clear();
        foreach (PreviewField previewField in previewFields)
        {
            if (previewField is TextPreviewField field)
            {
                DescriptionsItem item = new DescriptionsItem() {FontWeight = FontWeight.Bold, Label = new Skeleton() {Content = field.Label, IsActive = true, IsLoading = true} , Content = new Skeleton() {Content = new TextBlock() {Text = field.Text, FontWeight = FontWeight.Regular}, IsActive = true, IsLoading = true}};
                this._descriptionsFields.Items.Add(item);
            }
            else if (previewField is StatusPreviewField field2)
            {
                Label label = new Label() {Content = field2.Text, Classes = { "Solid", field2.Color }, Theme = this.FindResource("TagLabel") as ControlTheme};
                DescriptionsItem item = new DescriptionsItem() { FontWeight = FontWeight.Bold, Label = new Skeleton() {Content = field2.Label, IsActive = true, IsLoading = true}, Content = new Skeleton() {Content = label, IsActive = true, IsLoading = true} };
                this._descriptionsFields.Items.Add(item);
            }
            else if (previewField is PlayersPreviewField field3)
            {
                DescriptionsItem item = new DescriptionsItem() { FontWeight = FontWeight.Bold, Label = new Skeleton() {Content = field3.Label, IsActive = true, IsLoading = true}, Content = new Skeleton() {Content = new TextBlock() {Text = $"{field3.CurrentNumber} / {field3.MaxNumber}", FontWeight = FontWeight.Bold }, IsActive = true, IsLoading = true}};
                this._descriptionsFields.Items.Add(item);
            }
        }
    }

    private void _setLoading(bool value)
    {
        foreach (var item in this._descriptionsFields.Items)
        {
            if (item is DescriptionsItem field)
            {
                if (field.Label is Skeleton sl) sl.IsLoading = value;
                if (field.Content is Skeleton sc) sc.IsLoading = value;
            }
        }
    }

    public void ShowLoading() => this._setLoading(true);
    public void HideLoading() => this._setLoading(false);
}