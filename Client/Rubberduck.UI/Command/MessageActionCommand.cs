using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;

namespace Rubberduck.UI.Command
{
    /// <summary>
    /// A command that receives a <c>Window</c> as a parameter, closes it, and sets the corresponding <c>MessageAction</c> as <c>IDialogViewModel.SelectedAction</c> .
    /// </summary>
    public abstract class MessageActionCommand : CommandBase
    {
        protected MessageActionCommand(ILogger logger, ISettingsProvider<RubberduckSettings> settings, MessageAction messageAction) 
            : base(logger, settings)
        {
            MessageAction = messageAction;
        }

        public MessageAction MessageAction { get; init; }

        protected override async Task OnExecuteAsync(object? parameter)
        {
            var view = parameter as Window ?? throw new ArgumentException("Parameter was not a window.", nameof(parameter));
            var viewModel = view.DataContext as IDialogWindowViewModel ?? throw new ArgumentException($"Type '{view.DataContext?.GetType()}' does not implement {nameof(IDialogWindowViewModel)}.");

            SystemCommands.CloseWindow(view);
            viewModel.SelectedAction = MessageAction;

            await Task.CompletedTask;
        }
    }

    public class AcceptMessageActionCommand : MessageActionCommand
    {
        public AcceptMessageActionCommand(ILogger logger, ISettingsProvider<RubberduckSettings> settings, MessageAction? acceptAction = null, Func<object?, bool>? validations = null)
            : base(logger, settings, acceptAction ?? MessageAction.AcceptAction)
        {
            if (validations is not null)
            {
                AddToCanExecuteEvaluation(validations);
            }
        }
    }

    public class CloseMessageActionCommand : MessageActionCommand
    {
        public CloseMessageActionCommand(ILogger logger, ISettingsProvider<RubberduckSettings> settings, MessageAction? closeAction = null) 
            : base(logger, settings, closeAction ?? MessageAction.CloseAction)
        {
        }
    }

    public class CancelMessageActionCommand : MessageActionCommand
    {
        public CancelMessageActionCommand(ILogger logger, ISettingsProvider<RubberduckSettings> settings, MessageAction? cancelAction = null) 
            : base(logger, settings, cancelAction ?? MessageAction.CancelAction)
        {
        }
    }
}
