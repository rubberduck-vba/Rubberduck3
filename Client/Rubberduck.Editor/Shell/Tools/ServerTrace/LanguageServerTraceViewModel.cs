using Rubberduck.Editor.Commands;
using Rubberduck.SettingsProvider.Model.Editor.Tools;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Services;
using Rubberduck.UI.Shell.Tools.ServerTrace;

namespace Rubberduck.Editor.Shell.Tools.ServerTrace
{
    public class LanguageServerTraceViewModel : ServerTraceViewModel, ILanguageServerTraceViewModel
    {
        public LanguageServerTraceViewModel(UIServiceHelper service,
            ShowRubberduckSettingsCommand showSettingsCommand,
            CloseToolWindowCommand closeToolWindowCommand,
            OpenLogFileCommand openLogFileCommand,
            ShutdownServerCommand shutdownCommand)
            : base(service, showSettingsCommand, closeToolWindowCommand, openLogFileCommand, shutdownCommand)
        {
            Title = "Language Server Trace";
            SettingKey = nameof(ServerTraceSettings);
        }
    }
}
