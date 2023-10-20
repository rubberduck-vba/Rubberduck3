using System;
using System.Windows;
using Microsoft.Extensions.Logging;
using Rubberduck.Editor.Message;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;

namespace Rubberduck.Editor.Command
{
    /// <summary>
    /// A command that receives a <c>Window</c> as a parameter, closes it, and broadcasts a <c>MessageActionResult</c>.
    /// </summary>
    public abstract class MessageActionCommandBase : CommandBase
    {
        protected MessageActionCommandBase(ILogger logger, ISettingsProvider<RubberduckSettings> settings, MessageAction messageAction) 
            : base(logger, settings)
        {
            MessageAction = messageAction;
        }

        private MessageAction MessageAction { get; init; }

        protected sealed override void OnExecute(object? parameter)
        {
            var view = parameter as Window ?? throw new ArgumentException("Parameter was not a window.", nameof(parameter));
            var viewModel = view.DataContext as IDialogWindowViewModel ?? throw new ArgumentException($"Type '{view.DataContext?.GetType()}' does not implement {nameof(IDialogWindowViewModel)}.");

            SystemCommands.CloseWindow(view);
            viewModel.SelectedAction = MessageAction;
        }
    }

    public class AcceptMessageActionCommand : MessageActionCommandBase
    {
        public AcceptMessageActionCommand(ILogger logger, ISettingsProvider<RubberduckSettings> settings, Func<object?, bool>? validations = null)
            : base(logger, settings, MessageAction.AcceptAction)
        {
            if (validations is not null)
            {
                AddToCanExecuteEvaluation(validations);
            }
        }
    }

    public class CloseMessageActionCommand : MessageActionCommandBase
    {
        public CloseMessageActionCommand(ILogger logger, ISettingsProvider<RubberduckSettings> settings) 
            : base(logger, settings, MessageAction.CloseAction)
        {
        }
    }

    public class CancelMessageActionCommand : MessageActionCommandBase
    {
        public CancelMessageActionCommand(ILogger logger, ISettingsProvider<RubberduckSettings> settings) 
            : base(logger, settings, MessageAction.CancelAction)
        {
        }
    }
}
