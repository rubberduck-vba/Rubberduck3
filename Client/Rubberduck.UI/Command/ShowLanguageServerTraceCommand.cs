using Rubberduck.UI.Services;
using Rubberduck.UI.Shell.Tools.ServerTrace;

namespace Rubberduck.UI.Command
{
    public class ShowLanguageServerTraceCommand : ShowToolWindowCommand<ServerTraceControl, ILanguageServerTraceViewModel>
    {
        public ShowLanguageServerTraceCommand(UIServiceHelper service, ShellProvider shell, ILanguageServerTraceViewModel vm)
            : base(service, shell, vm)
        {
        }
    }
}
