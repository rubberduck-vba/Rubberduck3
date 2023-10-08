using Rubberduck.Settings;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.VersionCheck
{
    public interface IVersionCheckService
    {
        Task<Version?> GetLatestVersionAsync(CancellationToken token = default);
        Version CurrentVersion { get; }
        bool IsDebugBuild { get; }
        string VersionString { get; }
    }
}