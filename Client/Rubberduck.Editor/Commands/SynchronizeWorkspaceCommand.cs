using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using System;
using System.Threading.Tasks;

namespace Rubberduck.Editor.Commands
{
    public class SynchronizeWorkspaceCommand : CommandBase
    {
        public SynchronizeWorkspaceCommand(UIServiceHelper service)
            : base(service)
        {
            // FIXME needs to know whether there's a connected add-in client.
            AddToCanExecuteEvaluation(param => false);
        }

        protected override Task OnExecuteAsync(object? parameter)
        {
            throw new NotImplementedException();
        }
    }
}
