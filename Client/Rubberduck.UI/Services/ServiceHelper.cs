using Microsoft.Extensions.Logging;
using Rubberduck.SettingsProvider;

namespace Rubberduck.UI.Services
{
    public class ServiceHelper : ServiceBase
    {
        public ServiceHelper(ILogger<ServiceHelper> logger, RubberduckSettingsProvider settingsProvider) 
            : base(logger, settingsProvider)
        {
        }
    }
}