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
            OpenLogFileCommand openLogFileCommand)
            : base(service, showSettingsCommand, closeToolWindowCommand, openLogFileCommand)
        {
            Title = "Language Server Trace";
            //SettingKey = nameof(LanguageServerTraceToolSettings);
        }
    }
}
