using Rubberduck.UI.Command;
using Rubberduck.UI.Command.SharedHandlers;
using System.Collections.Generic;
using System.Windows.Input;

namespace Rubberduck.Editor.Commands
{
    public class ToolsCommandHandlers
    {
        public ToolsCommandHandlers(ShowRubberduckSettingsCommand optionsCommand)
        {
            OptionsCommand = optionsCommand;
        }

        public ICommand OptionsCommand { get; init; }

        public IEnumerable<CommandBinding> CreateCommandBindings() => new[]
        {
            new CommandBinding(ToolsCommands.ShowRubberduckSettingsCommand, OptionsCommandBinding_Executed, OptionsCommandBinding_CanExecute),
        };

        private void OptionsCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = OptionsCommand.CanExecute(e.Parameter);
        private void OptionsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
            => OptionsCommand.Execute(e.Parameter);
    }
}
