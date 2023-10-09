using Microsoft.Extensions.Logging;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.Unmanaged.Abstract;

namespace Rubberduck.UI.Command
{
    public abstract class ComCommandBase : CommandBase
    {
        private readonly IVbeEvents _vbeEvents;

        protected ComCommandBase(ILogger logger, ISettingsProvider<RubberduckSettings> settingsProvider, IVbeEvents vbeEvents)
            : base(logger, settingsProvider)
        {
            _vbeEvents = vbeEvents;
            AddToCanExecuteEvaluation(SpecialEvaluateCanExecute, true);
        }

        private bool SpecialEvaluateCanExecute(object? parameter)
        {
            return !_vbeEvents.Terminated;
        }
    }
}
