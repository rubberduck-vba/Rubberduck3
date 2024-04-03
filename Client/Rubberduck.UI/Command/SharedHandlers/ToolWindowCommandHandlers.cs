using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Command.StaticRouted;
using System.Collections.Generic;
using System.Windows.Input;

namespace Rubberduck.UI.Command.SharedHandlers
{
    public class ToolWindowCommandHandlers : CommandHandlers
    {
        private readonly ICommand _closeCommand;

        public ToolWindowCommandHandlers(CloseToolWindowCommand closeCommand)
        {
            _closeCommand = closeCommand;
        }

        public override IEnumerable<CommandBinding> CreateCommandBindings()
        {
            return Bind((ToolsCommands.CloseToolwindowCommand, _closeCommand));
        }
    }
}
