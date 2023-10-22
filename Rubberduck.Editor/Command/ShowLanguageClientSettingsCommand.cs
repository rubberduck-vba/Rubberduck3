using System;
using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Command;
using System.Threading.Tasks;

namespace Rubberduck.Editor.Command
{
    public class ShowLanguageClientSettingsCommand : CommandBase
    {
        public ShowLanguageClientSettingsCommand(ILogger logger, ISettingsProvider<RubberduckSettings> settings) 
            : base(logger, settings)
        {
        }

        protected override Task OnExecuteAsync(object? parameter)
        {
            throw new NotImplementedException();
        }
    }
}
