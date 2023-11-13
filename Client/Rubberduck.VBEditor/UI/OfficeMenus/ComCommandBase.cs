using Rubberduck.UI.Services;
using Rubberduck.Unmanaged.Abstract;

namespace Rubberduck.UI.Command
{
    public abstract class ComCommandBase : CommandBase
    {
        private readonly IVbeEvents _vbeEvents;

        protected ComCommandBase(ServiceHelper service, IVbeEvents vbeEvents)
            : base(service)
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
