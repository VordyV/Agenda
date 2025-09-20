using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Agenda.App.Items;
using Agenda.App.Models;
using LiteDB;

namespace Agenda.App.Services;

public class ConfigService
{

    private LiteDatabase _database;
    private ILiteCollection<ServerItem> _collectionServer;
    
    public ConfigService(string filename)
    {
        _database = new LiteDatabase(filename);
        _collectionServer = _database.GetCollection<ServerItem>("servers");
    }

    public List<ServerItem> GetAll() => _collectionServer.FindAll().ToList();
    public void Add(ServerItem server) => _collectionServer.Insert(server);
    public void Update(ServerItem server) => _collectionServer.Update(server);
    public void Delete(int id) => _collectionServer.Delete(id);
}