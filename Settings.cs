using System.Collections.Generic;
using Agenda.Core;
using Agenda.Core.ModelFieldControls;
using Avalonia.Controls;
using SimpleModule = Agenda.Modules.SimpleModule;
using RconBF2142DefaultModule = Agenda.Modules.RconBF2142DefaultModule;

namespace Agenda;

public static class Settings
{
    public static List<Module> Modules = new List<Module>()
    {
        new Module(
            id: "simple",
            title: "Simple",
            version: "0.1",
            view: (driver) => new SimpleModule.View(driver: driver),
            fields: 
            [
                new ModuleField(id: "address", title: "Address", control: () => new IPv4FieldControl(), required: true, validator: (data) => new ModuleFieldValidator(length: 15).Validate(data)),
                new ModuleField(id: "query_port", title: "Query port", control: () => new IntFieldControl(min: 1.0m, max: 65535.0m), required: true, validator: (data) => new ModuleFieldValidator(maxNum: 65535, minNum: 1).Validate(data))
            ],
            driver: (id, moduleId, fields) => new SimpleModule.Driver(id, moduleId, fields)
        ),
        new Module(
            id: "rconbf2142default",
            title: "Rcon BF2142 Default",
            version: "0.1",
            view: (driver) => new RconBF2142DefaultModule.View(),
            fields: 
            [
                new ModuleField(id: "address", title: "Address", control: () => new IPv4FieldControl(), required: true, validator: (data) => new ModuleFieldValidator(length: 15).Validate(data)),
                new ModuleField(id: "rcon_port", title: "Rcon port", control: () => new IntFieldControl(min: 1.0m, max: 65535.0m), required: true, validator: (data) => new ModuleFieldValidator(maxNum: 65535, minNum: 1).Validate(data)),
                new ModuleField(id: "rcon_password", title: "Rcon password", control: () => new PasswordFieldControl(), required: true, validator: (data) => new ModuleFieldValidator(length: 128).Validate(data))
            ],
            driver: (id, moduleId, fields) => new RconBF2142DefaultModule.Driver(id, moduleId, fields)
        )
    };
}