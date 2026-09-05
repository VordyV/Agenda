using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Velopack;
using Velopack.Sources;

namespace Agenda;

public class Updater
{
    private readonly UpdateManager _manager;

    public Updater(string url, bool prerelease)
    {
        this._manager = new UpdateManager(new GithubSource(url, accessToken: null, prerelease: prerelease));
    }
    
    public async Task<UpdateInfo?> CheckUpdate()
    {
        return await _manager.CheckForUpdatesAsync();
    }

    public async Task Update(UpdateInfo updateInfo)
    {
        await _manager.DownloadUpdatesAsync(updateInfo);
        _manager.ApplyUpdatesAndRestart(updateInfo);
    }
}