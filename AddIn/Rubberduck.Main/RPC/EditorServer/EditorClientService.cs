using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NLog.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.ServerPlatform;
using System.Collections.Generic;

namespace Rubberduck.Main.RPC.EditorServer
{
    public class EditorClientService : ServerPlatformClientService
    {
        public EditorClientService(ILogger<ServerPlatformServiceHelper> logger, RubberduckSettingsProvider settings, IWorkDoneProgressStateService workdone, PerformanceRecordAggregator performance) 
            : base(logger, settings, workdone, performance)
        {
        }

        protected override void ConfigureClientLogging(ILoggingBuilder builder)
        {
            builder.AddNLog("NLog-addin.config");
        }

        protected override ClientCapabilities GetClientCapabilities()
        {
            var capabilities = new ClientCapabilities
            {
                Workspace = new WorkspaceClientCapabilities
                {
                    Configuration = Supported,
                    ApplyEdit = Supported,
                    WorkspaceFolders = Supported,

                    DidChangeConfiguration = new()
                    {
                        Value = new()
                    },
                    ExecuteCommand = new()
                    {
                        Value = new()
                    },
                    FileOperations = new()
                    {
                        Value = new()
                        {
                            DidCreate = Supported,
                            DidDelete = Supported,
                            DidRename = Supported, // really?
                        }
                    },
                    WorkspaceEdit = new()
                    {
                        Value = new()
                        {
                            DocumentChanges = Supported, // NOTE: TextEdit is unhandled
                            FailureHandling = FailureHandlingKind.Abort,
                            NormalizesLineEndings = Supported,
                            //ResourceOperations = supported
                        }
                    },
                    ExtensionData = new Dictionary<string, JToken>()
                },
                Window = null,
                TextDocument = null,

                //Experimental = new Dictionary<string, JToken>(),
                //ExtensionData = new Dictionary<string, JToken>()
            };

            return capabilities;
        }
    }
}
