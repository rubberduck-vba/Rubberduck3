using Rubberduck.UI.LanguageServerTrace;
using Rubberduck.UI.Services;

namespace Rubberduck.UI.Command
{
    public class ShowLanguageServerTraceCommand : ShowToolWindowCommand<ServerTraceControl, LanguageServerTraceViewModel>
    {
        public ShowLanguageServerTraceCommand(UIServiceHelper service, ShellProvider shell, LanguageServerTraceViewModel vm)
            : base(service, shell, vm)
        {
        }
    }
}
