using System;
using Rubberduck.Editor.Message;

namespace Rubberduck.Editor.Command
{
    public class MessageActionExecutedEventArgs : EventArgs
    {
        public MessageActionExecutedEventArgs(MessageActionResult result)
        {
            Result = result;
        }

        public MessageActionResult Result { get; init;}
    }
}
