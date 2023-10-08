//using IndenterSettings = Rubberduck.SmartIndenter.IndenterSettings;
using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;

namespace Rubberduck.Root
{
    public class LogLevelService : ILogLevelService
    {
        public void SetMinimumLogLevel(LogLevel level) 
        {
            //TODO port from RD2: LogLevelHelper.SetMinimumLogLevel(LogLevel.FromOrdinal(_config.UserSettings.GeneralSettings.MinimumLogLevel));
        }
    }
}
