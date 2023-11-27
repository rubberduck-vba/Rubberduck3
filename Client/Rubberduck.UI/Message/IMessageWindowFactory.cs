using Rubberduck.UI.Command;
using Rubberduck.UI.Command.SharedHandlers;
using System;

namespace Rubberduck.UI.Message
{
    public interface IMessageWindowFactory
    {
        (MessageWindow view, IMessageWindowViewModel viewModel) Create<TModel>(TModel model, Func<MessageActionsProvider, MessageActionCommand[]>? actions = null) where TModel : MessageModel;
    }

    public class MessageWindowFactory : IMessageWindowFactory
    {
        private readonly MessageActionsProvider _provider;

        public MessageWindowFactory(MessageActionsProvider provider)
        {
            _provider = provider;
        }

        public (MessageWindow view, IMessageWindowViewModel viewModel) Create<TModel>(TModel model, Func<MessageActionsProvider, MessageActionCommand[]>? actions = null) where TModel : MessageModel
        {
            var buttons = actions?.Invoke(_provider) ?? _provider.OkOnly();

            var viewModel = new MessageWindowViewModel(model, buttons);
            var view = new MessageWindow(viewModel)
            {
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Height = 320,
                Width = 600,
            };
            return (view, viewModel);
        }
    }
}
