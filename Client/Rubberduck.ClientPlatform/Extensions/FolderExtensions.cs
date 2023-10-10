using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Rubberduck.Client.Extensions
{
    public static class FolderExtensions
    {
        public static WorkspaceFolder ToWorkspaceFolder(this System.IO.DirectoryInfo folder)
        {
            return new WorkspaceFolder
            {
                Name = folder.Name,
                Uri = folder.FullName
            };
        }
    }
}
