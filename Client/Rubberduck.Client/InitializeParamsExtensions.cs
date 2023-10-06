using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.SettingsProvider.Model;

namespace Rubberduck.Client
{
    public static class InitializeParamsExtensions
    {
        public static InitializeParams ConfigureInitialization(this InitializeParams request, long clientProcessId, string locale)
        {
            var type = request.GetType();
            type.GetProperty(nameof(request.ProcessId))!.SetValue(request, clientProcessId);
            type.GetProperty(nameof(request.Locale))!.SetValue(request, locale);

            return request;
        }

        public static InitializeTrace ToInitializeTrace(this ServerTraceLevel trace)
        {
            if (trace == ServerTraceLevel.Off)
            {
                return InitializeTrace.Off;
            }

            if (trace == ServerTraceLevel.Verbose)
            {
                return InitializeTrace.Verbose;
            }

            return InitializeTrace.Messages;
        }
    }
}
