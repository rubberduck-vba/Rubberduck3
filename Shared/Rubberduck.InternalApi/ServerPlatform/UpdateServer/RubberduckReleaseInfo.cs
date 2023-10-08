using System;

namespace Rubberduck.InternalApi.ServerPlatform.UpdateServer
{
    public readonly struct RubberduckReleaseInfo
    {
        public string TagName { get; init; }
        public string Version { get; init; }

        public bool IsPrerelease { get; init; }
        public DateTime ReleaseDate { get; init; }
        public string ReleaseNotesUrl { get; init; }
        public string DownloadUrl { get; init; }

        public int InstallerDownloads { get; init; }
    }
}
