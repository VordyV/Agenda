using System.IO.Compression;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using Agenda.Core.ModelFieldControls;

namespace Agenda.Core;

public sealed class ModuleLoadContext : AssemblyLoadContext
{
    public ModuleLoadContext(string name) : base(name, isCollectible: true) { }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        Assembly coreAssembly = typeof(ModuleLoadContext).Assembly;

        if (AssemblyName.ReferenceMatchesDefinition(assemblyName, coreAssembly.GetName())) return coreAssembly;
        
        try
        {
            return Default.LoadFromAssemblyName(assemblyName);
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }
}

public class ModuleData
{
    public ModuleManifest Manifest { get; set; }
    public Assembly Context { get; set; }
}

class ModuleLoader
{
    private AgendaCore _core;
    private string _dir;
    private string _extension;
    private string _manifest_filename;
    private Dictionary<string, ModuleData> _modules;

    public ModuleLoader(AgendaCore core, string dir, string extension, string manifest_filename)
    {
        this._core = core;
        this._dir = dir;
        this._extension = extension;
        this._manifest_filename = manifest_filename;
    }

    private BaseModelFieldControl CreateModelFieldControl(string type)
    {
        if (type == "text") return new TextFieldControl();
        if (type == "integer") return new IntFieldControl();
        if (type == "ipv4") return new IPv4FieldControl();
        if (type == "password") return new PasswordFieldControl();
        else throw new Exception($"Invalid field type: '{type}'");
    }
    
    private async Task InitModule(ModuleData moduleData)
    {
        Type? asmTypeView = moduleData.Context.GetType(moduleData.Manifest.view);
        if (asmTypeView is null) throw new Exception($"Module '{moduleData.Manifest.id}' has no view '{moduleData.Manifest.view}'");

        Type? asmTypeDriver = moduleData.Context.GetType(moduleData.Manifest.driver);
        if (asmTypeDriver is null) throw new Exception($"Module '{moduleData.Manifest.id}' has no driver '{moduleData.Manifest.driver}'");

        List<ModuleField> fields = new();
        foreach (var field in moduleData.Manifest.fields)
        {
            fields.Add(new ModuleField(id: field.id, title: field.title, control: () => this.CreateModelFieldControl(field.type), required: field.required, validator: (data) => new ModuleFieldValidator(length: field.length, maxNum: field.max_number, minNum: field.min_number).Validate(data), value: field.value));
        }

        List<Tag> tags = new();
        foreach (var tag in moduleData.Manifest.tags)
        {
            tags.Add(new Tag() {Color = tag.color, Text = tag.text});
        }
        
        Module module = new Module(
            id: moduleData.Manifest.id,
            title: moduleData.Manifest.title,
            version: moduleData.Manifest.version,
            description: moduleData.Manifest.description,
            subtitleFormat: moduleData.Manifest.subtitle_format,
            preview: moduleData.Manifest.preview,
            numberPreviewFields: moduleData.Manifest.number_preview_fields,
            fields: fields,
            view: conn => (BasicView)Activator.CreateInstance(asmTypeView, conn),
            driver: asmTypeDriver,
            tags: tags.ToArray()
        );

        this._core.RegisterModule(module);
    }

    private async Task<ModuleData> ModulePreparation(string path)
    {
        ModuleData moduleData = new ModuleData();
        using (ZipArchive archive = ZipFile.OpenRead(path))
        {
            ZipArchiveEntry? entry = archive.GetEntry(this._manifest_filename);
            if (entry is null) throw new Exception("failed to retrieve the manifest file");
            
            using (Stream stream = entry.Open())
            {
                ModuleManifest? manifest = await JsonSerializer.DeserializeAsync<ModuleManifest>(stream);
                if (manifest is null) throw new Exception("failed to serialize the manifest data");
                moduleData.Manifest = manifest;
            }

            if (moduleData.Manifest.type == 1)
            {
                entry = archive.GetEntry($"{moduleData.Manifest.target}.dll");
                if (entry is null) throw new Exception($"failed to retrieve the module target '{moduleData.Manifest.target}.dll'");

                using (Stream entryStream = entry.Open())
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    entryStream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    ModuleLoadContext context = new ModuleLoadContext($"Module{moduleData.Manifest.id}");
                    moduleData.Context = context.LoadFromStream(memoryStream);
                }
            }
            else throw new Exception("module type in the manifest has an invalid value");
        }
        return moduleData;
    }

    public async Task LoadModule(string path)
    {
        ModuleData moduleData = await this.ModulePreparation(path);
        await this.InitModule(moduleData);
    }

    public async Task<string> LoadProtoModule(string dir)
    {
        ModuleData moduleData = await this.ProtoModulePreparation(dir);
        await this.InitModule(moduleData);
        return moduleData.Manifest.id;
    } 

    public async Task LoadModules()
    {
        string[] files = Directory.GetFiles(this._dir);
        foreach (var file in files)
        {
            if (Path.GetExtension(file) != this._extension) continue;
            try
            {
                await this.LoadModule(file);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Module '{file}' loading error: {e.Message}");
                Console.WriteLine(e.StackTrace);
            }
        }
    }

    public async Task<ModuleData> ProtoModulePreparation(string dir)
    {
        string pathManifest = Path.Combine(dir, this._manifest_filename);
        if (!File.Exists(pathManifest)) throw new Exception($"failed to retrieve the manifest file: {pathManifest}");
        string? rowDataManifest = await File.ReadAllTextAsync(pathManifest);
        
        ModuleManifest? manifest = JsonSerializer.Deserialize<ModuleManifest>(rowDataManifest);
        if (manifest is null) throw new Exception("failed to serialize the manifest data");
        ModuleData moduleData = new ModuleData();
        moduleData.Manifest = manifest;
        
        if (moduleData.Manifest.type == 1)
        {
            string pathTarget = Path.Combine(dir, $"{moduleData.Manifest.target}.dll");
            if (!File.Exists(pathTarget)) throw new Exception($"failed to retrieve the module target '{moduleData.Manifest.target}.dll'");
            
            using (FileStream fstream = File.OpenRead(pathTarget))
            using (MemoryStream memoryStream = new MemoryStream())
            {
                fstream.CopyTo(memoryStream);
                memoryStream.Position = 0;
                ModuleLoadContext context = new ModuleLoadContext($"Module{moduleData.Manifest.id}");
                moduleData.Context = context.LoadFromStream(memoryStream);
            }
        }
        else throw new Exception("module type in the manifest has an invalid value");

        return moduleData;
    }
}