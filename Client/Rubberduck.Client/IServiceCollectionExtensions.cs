using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Reflection;

namespace Rubberduck.Client
{
    public static class AssemblyExtensions
    {
        public static ClientInfo ToClientInfo(this Assembly assembly)
        {
            var name = assembly.GetName();
            return new ClientInfo
            {
                Name = name.Name,
                Version = name.Version.ToString(3)
            };
        }
    }

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
