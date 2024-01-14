using Microsoft.Extensions.Logging;
using Rubberduck.SettingsProvider;

namespace Rubberduck.ServerPlatform
{
    public class ServerPlatformServiceHelper : ServiceBase
    {
        public ServerPlatformServiceHelper(ILogger<ServerPlatformServiceHelper> logger, RubberduckSettingsProvider settings, IWorkDoneProgressStateService workdone, PerformanceRecordAggregator performance) 
            : base(logger, settings, workdone, performance) { }
    }
}