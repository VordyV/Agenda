using Agenda.Core;
using Agenda.Core.ModelFieldControls;
using SimpleModule;

namespace Manifest;

public class Main
{
    public Module Module = new Module(
        id: "simplemodule",
        title: "Simple Module",
        version: "0.1",
        description: "Simple test module",
        view: (conn) => new SimpleModuleView(conn),
        fields:
        [
            new ModuleField(id: "address", title: "Address", control: () => new IPv4FieldControl(), required: true, validator: (data) => new ModuleFieldValidator(length: 15).Validate(data)),
            new ModuleField(id: "query_port", title: "Query port", control: () => new IntFieldControl(min: 1.0m, max: 65535.0m), required: true, validator: (data) => new ModuleFieldValidator(maxNum: 65535, minNum: 1).Validate(data))
        ],
        driver: typeof(SimpleModuleDriver),
        subtitleFormat: "{address}:**{query_port}**",
        preview: true,
        numberPreviewFields: 1,
        tags: new Tag[]{}
    );
}