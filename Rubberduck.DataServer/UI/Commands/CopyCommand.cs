using NLog;
using Rubberduck.RPC.Proxy.SharedServices;
using Rubberduck.RPC.Proxy.SharedServices.Console.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using Rubberduck.UI.Abstract;
using Rubberduck.UI.Command;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Rubberduck.Client.LocalDb.UI.Commands
{
    public class CopyCommand : CommandBase
    {
        private readonly IServerConsoleService<ServerConsoleOptions> _console;

        public CopyCommand(IServerConsoleService<ServerConsoleOptions> console) 
            : base(LogManager.GetCurrentClassLogger())
        {
            _console = console;
        }

        protected override void OnExecute(object parameter)
        {
            _console.Log(ServerLogLevel.Info, $"Executing {nameof(CopyCommand)}...");

            var asText = new StringBuilder();
            if (parameter is IEnumerable<IConsoleMesssageViewModel> messages)
            {
                foreach (var (msg, i) in messages.Select((m, i) => (m, i)))
                {
                    asText.AppendLine(msg.ToClipboardString());
                }

                Clipboard.SetText(asText.ToString());
            }
        }
    }
}
