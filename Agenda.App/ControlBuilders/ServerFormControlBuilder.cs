using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;

namespace Agenda.App.ControlBuilders;

public class ServerFormControlBuilder : UserControl
{
    private StackPanel _uiStackPanel;
    private readonly Dictionary<string, IDriverField> _fieldControls = new();
    
    public ServerFormControlBuilder(Driver driver)
    {
        _uiStackPanel = new StackPanel();

        foreach (string fieldName in driver.Fields.Keys)
        {
            _fieldControls.Add(fieldName, driver.Fields[fieldName]());
            //_uiStackPanel.Children.Add(_fieldControls[fieldName].GetControl());
        }

        Content = _uiStackPanel;
    }

    public List<IDriverField> GetControls() => _fieldControls.Values.ToList();

    public Dictionary<string, string?> GetValues()
    {
        Dictionary<string, string?> result = new();
        
        foreach (var field in _fieldControls)
        {
            result.Add(field.Key, field.Value.GetValue());
        }

        return result;
    }

    public void SetError(string fieldName, string error)
    {
        var field = _fieldControls[fieldName];
        DataValidationErrors.SetErrors(field.GetControl(), new object[] { error });
    }
}