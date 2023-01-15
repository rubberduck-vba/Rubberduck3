using NLog;
using Rubberduck.RPC.Platform;
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
        private readonly IJsonRpcConsole _console;

        public CopyCommand(IJsonRpcConsole console) : base(LogManager.GetCurrentClassLogger())
        {
            _console = console;
        }

        protected override void OnExecute(object parameter)
        {
            _console.Log(LogLevel.Info, $"Executing {nameof(CopyCommand)}...");

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
