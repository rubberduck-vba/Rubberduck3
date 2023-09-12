using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi;
using System.IO.Pipelines;
using System.IO.Pipes;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

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

    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureLanguageClient(this IServiceCollection services, ClientInfo client, ClientCapabilities capabilities, WorkspaceFolder workspace)
        {
            var pipe = new NamedPipeClientStream(".", ServerPlatformSettings.LanguageServerPipeName, PipeDirection.InOut);
            return services
                .AddLanguageClient(options =>
                {
                    options.Services = services;
                    options.WithInput(pipe)
                           .WithOutput(pipe)
                           .WithClientInfo(client)
                           .WithContentModifiedSupport(true)
                           .WithWorkspaceFolder(workspace)
                           .WithClientCapabilities(capabilities)
                           .WithInitializationOptions(new { /* TODO add host app info */ })
                           .EnableProgressTokens()
                           .EnableWorkspaceFolders()
                    ;
                });
        }
    }
}
