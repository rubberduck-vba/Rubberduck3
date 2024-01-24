using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;

namespace Rubberduck.ServerPlatform
{
    public class ServerPlatformServiceHelper : ServerPlatformServiceBase
    {
        public ServerPlatformServiceHelper(ILogger<ServerPlatformServiceHelper> logger, RubberduckSettingsProvider settings, IWorkDoneProgressStateService workdone, PerformanceRecordAggregator performance) 
            : base(logger, settings, workdone, performance) { }
    }
}