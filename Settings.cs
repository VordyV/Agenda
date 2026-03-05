using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Agenda.Core;
using Agenda.Core.ModelFieldControls;
using Agenda.Modules.TCPCModule;
using Avalonia.Controls;
using Module = Agenda.Core.Module;
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
            description: "Simple test module",
            view: (conn) => new SimpleModule.SimpleView(conn),
            fields: 
            [
                new ModuleField(id: "address", title: "Address", control: () => new IPv4FieldControl(), required: true, validator: (data) => new ModuleFieldValidator(length: 15).Validate(data)),
                new ModuleField(id: "query_port", title: "Query port", control: () => new IntFieldControl(min: 1.0m, max: 65535.0m), required: true, validator: (data) => new ModuleFieldValidator(maxNum: 65535, minNum: 1).Validate(data))
            ],
            driver: (connId) => new SimpleModule.SimpleDriver(connId)
        ),
        new Module(
            id: "tcpc",
            title: "TCP Client",
            version: "0.1",
            description: "A simple client that receives and sends data via the TCP protocol",
            view: (conn) => new TcpcView(conn),
            fields: 
            [
                new ModuleField(id: "address", title: "Address", control: () => new IPv4FieldControl(), required: true, validator: (data) => new ModuleFieldValidator(length: 15).Validate(data)),
                new ModuleField(id: "port", title: "Port", control: () => new IntFieldControl(min: 1.0m, max: 65535.0m), required: true, validator: (data) => new ModuleFieldValidator(maxNum: 65535, minNum: 1).Validate(data))
            ],
            driver: (connId) => new TCPCDriver(connId)
        ),
        new Module(
            id: "rconbf2142default",
            title: "Rcon BF2142 Default",
            version: "0.1",
            description: "A client for remote control of a Battlefield 2142 game server via the RCON protocol, protocol module - Default",
            view: (conn) => new RconBF2142DefaultModule.RconBf2142DefaultView(conn),
            fields: 
            [
                new ModuleField(id: "address", title: "Address", control: () => new IPv4FieldControl(), required: true, validator: (data) => new ModuleFieldValidator(length: 15).Validate(data), value: IPAddress.Parse("127.0.0.1")),
                new ModuleField(id: "rcon_port", title: "Rcon port", control: () => new IntFieldControl(min: 1.0m, max: 65535.0m), required: true, validator: (data) => new ModuleFieldValidator(maxNum: 65535, minNum: 1).Validate(data), value: 4711),
                new ModuleField(id: "rcon_password", title: "Rcon password", control: () => new PasswordFieldControl(), required: true, validator: (data) => new ModuleFieldValidator(length: 128).Validate(data), value: "super123")
            ],
            driver: (connId) => new RconBF2142DefaultModule.RconBf2142DefaultDriver(connId)
        )
    };
    
    public static string GithubUrl = "https://github.com/VordyV/Agenda";
    public static string BugReportUrl = "https://github.com/VordyV/Agenda/issues/new";

    public static string TextAbout = $"""
                                     ## Agenda v{Assembly.GetExecutingAssembly().GetName().Version}

                                     **Developed by a human:** Vladislav Netievsky aka VordyV
                                     **Github:** {Settings.GithubUrl}
                                     
                                     To report errors and suggest improvements, please use the project's issue tracker on GitHub.
                                     
                                     #### Licensing
                                     Used third-party libraries are distributed in accordance with their licenses.
                                     
                                     Powered by Vorklab.space
                                     """;
}