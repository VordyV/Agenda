using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Agenda.App.ControlBuilders;
using Agenda.App.Items;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Agenda.App.Models;

public partial class ServerViewModel : ObservableObject, INotifyDataErrorInfo
{
    private Manager _manager;
    private PresenterView _presenterView;
    private ServerItem? _server;
    private readonly Dictionary<string, List<string>> _errors = new();

    public ObservableCollection<ServerTypeItem> Types { get; } = new();

    [ObservableProperty]
    private string _title;
    
    [ObservableProperty]
    private ServerTypeItem? _selectedServerType;

    [ObservableProperty]
    private ServerFormBuilder _serverFormBuilder;

    [ObservableProperty]
    //[MinLength(3, ErrorMessage = "name must be longer than 2 characters")]
    //[MaxLength(100, ErrorMessage = "name must be less than 100 characters long")]
    private string? _nameField;

    [ObservableProperty]
    private bool _isEnableType;
    
    [ObservableProperty]
    private bool _isVisibleRemove;
    
    public ServerViewModel(Manager manager, PresenterView presenterView, object? server)
    {
        _manager = manager;
        _presenterView = presenterView;
        _server = server != null ? (ServerItem)server : null;
        
        foreach (var driver in _manager.Drivers)
        {
            Types.Add(new ServerTypeItem() {Id = driver.Key, Name = driver.Value.Name});
        }

        if (_server == null)
        {
            Title = "Add a new server";
            SelectedServerType = Types[0];
            IsEnableType = true;
            IsVisibleRemove = false;
        }
        else
        {
            Title = $"Changing server data - {_server.Name}";
            SelectedServerType = Types.FirstOrDefault(x=> x.Id == _server.Type);
            IsEnableType = false;
            IsVisibleRemove = true;

            NameField = _server.Name;
            ServerFormBuilder.SetValues(_server.Options);
        }
    }

    [RelayCommand]
    public void GoServers() => _presenterView.LoadNew("servers");
    
    partial void OnSelectedServerTypeChanged(ServerTypeItem? item)
    {
        ServerFormBuilder = new ServerFormBuilder(_manager.Drivers[item!.Id]);
    }

    [RelayCommand]
    public void OnClickCancel() => _presenterView.LoadNew("servers");
    
    [RelayCommand]
    public void OnClickSave()
    {
        if (!Validate()) return;

        try
        {
            if (_server == null) _manager.Config.Add(new ServerItem() {Name = NameField, Type = SelectedServerType.Id, Options = ServerFormBuilder.GetValues()});
            else
            {
                _server.Name = NameField;
                _server.Options = ServerFormBuilder.GetValues();
                _manager.Config.Update(_server);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        _presenterView.LoadNew("servers");
    }

    [RelayCommand]
    public void OnClickRemove()
    {
        if (_server == null) return;
        
        _manager.Config.Delete(_server.Id);
        _presenterView.LoadNew("servers");
    }

    private bool Validate()
    {
        ClearErrors(nameof(NameField));
        
        if (string.IsNullOrWhiteSpace(NameField))
        {
            AddError(nameof(NameField), "name cannot be empty");
            return false;
        }

        if (NameField.Length > 100)
        {
            AddError(nameof(NameField), "name must be longer than 100 characters");
            return false;
        }
        
        if (NameField.Length < 3)
        {
            AddError(nameof(NameField), "name must be less than 3 characters long");
            return false;
        }
        
        return true;
    }
    
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public bool HasErrors => _errors.Count > 0;

    public IEnumerable GetErrors(string? propertyName)
    {
        if (propertyName is not null && _errors.TryGetValue(propertyName, out var errs))
            return errs;
        return Array.Empty<string>();
    }

    private void AddError(string propertyName, string error)
    {
        if (!_errors.TryGetValue(propertyName, out var errs))
            _errors[propertyName] = errs = new List<string>();
        errs.Add(error);
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    private void ClearErrors(string propertyName)
    {
        if (_errors.Remove(propertyName))
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }
}