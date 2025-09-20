using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Agenda.App.ControlBuilders;

public class ServerFormBuilder : UserControl
{
    private UniformGrid _uiUniformGrid;
    private readonly Dictionary<string, IDriverField> _fieldControls = new();
    
    public ServerFormBuilder(Driver driver)
    {
        _uiUniformGrid = new UniformGrid()
        {
            Columns = 2,
            ColumnSpacing = 8,
            RowSpacing = 8
        };

        foreach (string fieldName in driver.Fields.Keys)
        {
            _fieldControls.Add(fieldName, driver.Fields[fieldName]());
            _uiUniformGrid.Children.Add(_fieldControls[fieldName].GetControl());
        }

        Content = _uiUniformGrid;
    }

    //public List<IDriverField> GetControls() => _fieldControls.Values.ToList();

    public Dictionary<string, string?> GetValues()
    {
        Dictionary<string, string?> result = new();
        
        foreach (var field in _fieldControls)
        {
            result.Add(field.Key, field.Value.GetValue());
        }

        return result;
    }

    public void SetValues(Dictionary<string, string?> options)
    {
        foreach (var option in options)
        {
            var field = _fieldControls[option.Key];
            field.SetValue(option.Value);
        }
    }

    public void SetError(string fieldName, string error)
    {
        var field = _fieldControls[fieldName];
        DataValidationErrors.SetErrors(field.GetControl(), new object[] { error });
    }
}