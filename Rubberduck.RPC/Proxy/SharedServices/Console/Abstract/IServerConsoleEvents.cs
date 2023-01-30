using System;
using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;

namespace Rubberduck.RPC.Proxy.SharedServices.Console.Abstract
{
    public interface IServerConsoleEvents
    {
        /// <summary>
        /// An event that signals all messages to its registered handlers.
        /// </summary>
        [RubberduckSP("serverConsoleMessage")]
        event EventHandler<ConsoleMessage> Message;
    }
}
