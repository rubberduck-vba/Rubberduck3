using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Reflection;

namespace Rubberduck.Editor.RPC.LanguageServerClient
{
    public static class AssemblyExtensions
    {
        public static ClientInfo ToClientInfo(this Assembly assembly)
        {
            var name = assembly.GetName() ?? throw new ArgumentException("Could not get AssemblyName from specified assembly.", nameof(assembly));
            return new ClientInfo
            {
                Name = name.Name ?? throw new InvalidOperationException("Assembly name cannot be null."),
                Version = name.Version?.ToString(3) ?? throw new InvalidOperationException("Version cannot be null.")
            };
        }
    }
}
