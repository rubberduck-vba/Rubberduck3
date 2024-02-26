using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing.Exceptions;

namespace Rubberduck.LanguageServer
{
    public class SyntaxErrorMessageService : ServiceBase, ISyntaxErrorMessageService
    {
        public SyntaxErrorMessageService(ILogger<SyntaxErrorMessageService> logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance) 
            : base(logger, settingsProvider, performance)
        {
        }

        public bool TryGetMeaningfulMessage(AntlrSyntaxErrorInfo info, out string message)
        {
            /*TODO*/

            message = info.Message;
            return false;
        }
    }
}