using System.Windows.Input;
using Rubberduck.Editor.Message;

namespace Rubberduck.Editor.Command
{
    public record class MessageActionCommandBinding
    {
        public MessageActionCommandBinding(MessageAction messageAction, ICommand command)
        {
            MessageAction = messageAction;
            Command = command;
        }

        public MessageAction MessageAction { get; init; }
        public ICommand Command { get; init; }
    }
}
