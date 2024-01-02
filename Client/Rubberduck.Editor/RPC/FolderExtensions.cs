using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Model.Workspace;

namespace Rubberduck.Editor.RPC
{
    public static class FolderExtensions
    {
        public static WorkspaceFolder ToWorkspaceFolder(this System.IO.DirectoryInfo folder)
        {
            var path = folder.FullName.TrimEnd('\\');
            if (!path.EndsWith(ProjectFile.SourceRoot))
            {
                path = $"{path}\\{ProjectFile.SourceRoot}";
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
