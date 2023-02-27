using NLog;
using Rubberduck.RPC.Proxy.SharedServices.Console.Abstract;
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
        private readonly IServerConsoleProxyClient _console;

        public CopyCommand(IServerConsoleProxyClient console) 
            : base(LogManager.GetCurrentClassLogger())
        {
            _console = console;
        }

        protected override void OnExecute(object parameter)
        {
            // TODO set up client-side RPC server to send such console notifications
            //_console.Log(ServerLogLevel.Info, $"Executing {nameof(CopyCommand)}...");

            // ...otherwise TODO set up client-side RPC server to send telemetry notifications :)
            // _telemetry.OnEvent(...)

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
