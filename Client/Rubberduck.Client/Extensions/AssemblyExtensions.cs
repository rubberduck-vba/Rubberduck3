using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Reflection;

namespace Rubberduck.Client.Extensions
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
}
