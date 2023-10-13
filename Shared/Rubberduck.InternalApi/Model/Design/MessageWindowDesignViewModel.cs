using Rubberduck.InternalApi.Model.Abstract;
using System.ComponentModel;

namespace Rubberduck.InternalApi.Model.Design
{
    public class MessageWindowDesignViewModel : IMessageWindowViewModel
    {
        public string Title { get; } = "Title goes here";

        public string Message { get; } = "Message goes here.";

        public string Verbose { get; } = "Some verbose message details, or exception stack trace would go here.";

        public MessageAction? SelectedAction { get; set; }
        public MessageAction[] Actions { get; } = new[]
        {
            MessageAction.AcceptAction,
            MessageAction.CancelAction,
        };

        public event PropertyChangedEventHandler? PropertyChanged = delegate { };
    }
}
