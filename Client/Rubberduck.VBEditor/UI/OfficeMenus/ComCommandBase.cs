using Microsoft.Extensions.Logging;
using Rubberduck.Unmanaged.Abstract;

namespace Rubberduck.UI.Command
{
    public abstract class ComCommandBase : CommandBase
    {
        private readonly IVbeEvents _vbeEvents;

        protected ComCommandBase(ILogger logger, IVbeEvents vbeEvents)
            : base(logger)
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
