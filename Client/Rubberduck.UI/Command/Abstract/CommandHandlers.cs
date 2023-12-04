using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Rubberduck.UI.Command.Abstract
{
    public abstract class CommandHandlers
    {
        protected IEnumerable<CommandBinding> Bind(params (RoutedCommand routedCommand, ICommand command)[] bindings) => bindings
            .Select(e => new CommandBinding(e.routedCommand, 
                (sender, args) => e.command.Execute(args.Parameter), 
                (sender, args) => args.CanExecute = e.command.CanExecute(args.Parameter)));
        protected IEnumerable<CommandBinding> Bind(params (RoutedCommand routedCommand, Action<object> execute, Func<object, bool> canExecute)[] bindings) => bindings
            .Select(e => new CommandBinding(e.routedCommand,
                (sender, args) => e.execute(args.Parameter),
                (sender, args) => args.CanExecute = e.canExecute(args.Parameter)));
        protected IEnumerable<CommandBinding> Bind(params (RoutedCommand routedCommand, Action<object> execute)[] bindings) => bindings
            .Select(e => new CommandBinding(e.routedCommand,
                (sender, args) => e.execute(args.Parameter),
                (sender, args) => args.CanExecute = true));
        protected IEnumerable<CommandBinding> Bind(params (RoutedCommand routedCommand, ExecutedRoutedEventHandler execute, CanExecuteRoutedEventHandler canExecute)[] bindings) => bindings
            .Select(e => new CommandBinding(e.routedCommand,
                (sender, args) => e.execute(sender, args),
                (sender, args) => e.canExecute(sender, args)));

        public abstract IEnumerable<CommandBinding> CreateCommandBindings();
    }
}
