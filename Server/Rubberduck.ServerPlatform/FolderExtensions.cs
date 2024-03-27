using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Workspace;

namespace Rubberduck.ServerPlatform
{
    public static class FolderExtensions
    {
        public static WorkspaceFolder ToWorkspaceFolder(this System.IO.DirectoryInfo folder)
        {
            var path = folder.FullName.TrimEnd('\\');
            if (!path.EndsWith(WorkspaceUri.SourceRootName))
            {
                path = $"{path}\\{WorkspaceUri.SourceRootName}";
            }

            path += '\\';

            return new WorkspaceFolder
            {
                Name = folder.Name,
                Uri = path
            };
        }
    }
}
