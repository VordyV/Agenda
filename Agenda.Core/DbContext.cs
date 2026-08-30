using BLite.Bson;
using BLite.Core;
using BLite.Core.Collections;

namespace Agenda.Core;

public sealed partial class AppDbContext : DocumentDbContext
{
    public DocumentCollection<ObjectId, ProfileModel> Profiles { get; set; } = null!;

    public AppDbContext(string path) : base(path)
    {
        InitializeCollections();
    }
}