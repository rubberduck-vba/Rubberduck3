using Rubberduck.Editor.FileMenu;

namespace Rubberduck.Editor.Message
{
    public interface IMessageWindowFactory
    {
        (MessageWindow view, IMessageWindowViewModel viewModel) Create<TModel>(TModel model) where TModel : MessageModel;
    }

    public class MessageWindowFactory : IMessageWindowFactory
    {
        private readonly MessageActionsProvider _provider;

        public MessageWindowFactory(MessageActionsProvider provider) 
        {
            _provider = provider;
        }

        public (MessageWindow view, IMessageWindowViewModel viewModel) Create<TModel>(TModel model) where TModel : MessageModel
        {
            var viewModel = new MessageWindowViewModel(model, _provider);
            var view = new MessageWindow(viewModel);
            return (view, viewModel);
        }
    }
}
