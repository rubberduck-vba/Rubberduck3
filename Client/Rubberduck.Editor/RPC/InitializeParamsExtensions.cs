using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Settings;

namespace Rubberduck.Editor.RPC
{
    public static class InitializeParamsExtensions
    {
        public static InitializeParams ConfigureInitialization(this InitializeParams request, long clientProcessId, string locale, ProgressToken? progressToken = null)
        {
            var type = request.GetType();

            type.GetProperty(nameof(request.ProcessId))!.SetValue(request, clientProcessId);
            type.GetProperty(nameof(request.Locale))!.SetValue(request, locale);

            if (progressToken != null)
            {
                type.GetProperty(nameof(request.WorkDoneToken))!.SetValue(request, progressToken);
            }

            return request;
        }

        public static InitializeTrace ToInitializeTrace(this MessageTraceLevel trace)
        {
            if (trace == MessageTraceLevel.Off)
            {
                return InitializeTrace.Off;
            }

            if (trace == MessageTraceLevel.Verbose)
            {
                return InitializeTrace.Verbose;
            }

            return InitializeTrace.Messages;
        }
    }
}
