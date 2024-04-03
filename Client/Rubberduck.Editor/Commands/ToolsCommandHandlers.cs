using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Command.StaticRouted;
using System.Collections.Generic;
using System.Windows.Input;

namespace Rubberduck.Editor.Commands
{
    public class ToolsCommandHandlers : CommandHandlers
    {
        public ToolsCommandHandlers(ShowRubberduckSettingsCommand optionsCommand)
        {
            OptionsCommand = optionsCommand;
        }

        public ICommand OptionsCommand { get; init; }

        public override IEnumerable<CommandBinding> CreateCommandBindings() =>
            Bind(
                (ToolsCommands.ShowRubberduckSettingsCommand, OptionsCommand)
            );
    }
}
