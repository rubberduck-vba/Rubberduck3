using Rubberduck.UI.Command.Abstract;
using System.Collections.Generic;
using System.Windows.Input;

namespace Rubberduck.UI.Command.SharedHandlers;

public class NavigationCommandHandlers : CommandHandlers
{
    private readonly ICommand _navigateBackwardCommand;
    private readonly ICommand _navigateForwardCommand;
    private readonly ICommand _searchCommand;

    public NavigationCommandHandlers(
        ICommand navigateBackwardCommand, 
        ICommand navigateForwardCommand,
        ICommand searchCommand)
    {
        _navigateBackwardCommand = navigateBackwardCommand;
        _navigateForwardCommand = navigateForwardCommand;
        _searchCommand = searchCommand;
    }

    public override IEnumerable<CommandBinding> CreateCommandBindings() =>
        Bind(
            (NavigationCommands.BrowseBack, _navigateBackwardCommand),
            (NavigationCommands.BrowseForward, _navigateForwardCommand),
            (NavigationCommands.Search, _searchCommand)
        );
}