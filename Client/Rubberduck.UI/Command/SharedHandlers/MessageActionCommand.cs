using System;
using System.Threading.Tasks;
using System.Windows;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using Rubberduck.UI.Shared.Message;
using Rubberduck.UI.Windows;

namespace Rubberduck.UI.Command.SharedHandlers
{
    /// <summary>
    /// A command that receives a <c>Window</c> as a parameter, closes it, and sets the corresponding <c>MessageAction</c> as <c>IDialogViewModel.SelectedAction</c> .
    /// </summary>
    public abstract class MessageActionCommand : CommandBase
    {
        protected MessageActionCommand(UIServiceHelper service, MessageAction messageAction)
            : base(service)
        {
            MessageAction = messageAction;
        }

        public MessageAction MessageAction { get; init; }

        protected override async Task OnExecuteAsync(object? parameter)
        {
            var view = parameter as Window ?? throw new ArgumentException("Parameter was not a window.", nameof(parameter));
            var viewModel = view.DataContext as IDialogWindowViewModel ?? throw new ArgumentException($"Type '{view.DataContext?.GetType()}' does not implement {nameof(IDialogWindowViewModel)}.");

            view.DialogResult = MessageAction.IsDefaultAction;
            SystemCommands.CloseWindow(view);
            viewModel.SelectedAction = MessageAction;

            await Task.CompletedTask;
        }
    }

    public class AcceptMessageActionCommand : MessageActionCommand
    {
        public AcceptMessageActionCommand(UIServiceHelper service, MessageAction? acceptAction = null, Func<object?, bool>? validations = null)
            : base(service, acceptAction ?? MessageAction.AcceptAction)
        {
            if (validations is not null)
            {
                AddToCanExecuteEvaluation(validations);
            }
        }
    }

    public class CloseMessageActionCommand : MessageActionCommand
    {
        public CloseMessageActionCommand(UIServiceHelper service, MessageAction? closeAction = null)
            : base(service, closeAction ?? MessageAction.CloseAction)
        {
        }
    }

    public class CancelMessageActionCommand : MessageActionCommand
    {
        public CancelMessageActionCommand(UIServiceHelper service, MessageAction? cancelAction = null)
            : base(service, cancelAction ?? MessageAction.CancelAction)
        {
        }
    }
}
