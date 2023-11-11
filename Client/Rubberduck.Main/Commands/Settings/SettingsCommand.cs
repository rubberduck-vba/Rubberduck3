using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Command;
using Rubberduck.UI.Settings;
using Rubberduck.Unmanaged.Abstract;
using Rubberduck.VBEditor.UI.OfficeMenus;
using System.Threading.Tasks;

namespace Rubberduck.Main.Settings
{
    public class SettingsCommand : ComCommandBase, ISettingsCommand
    {
        private readonly ISettingsDialogService _service;
        private readonly ILanguageClientFacade _lsp;

        public SettingsCommand(ILogger<SettingsCommand> logger, ISettingsProvider<RubberduckSettings> settingsProvider, IVbeEvents vbeEvents, 
            ILanguageClientFacade lsp,
            ISettingsDialogService service)
            : base(logger, settingsProvider, vbeEvents)
        {
            _service = service;
            _lsp = lsp;

            settingsProvider.SettingsChanged += SettingsProvider_SettingsChanged;
        }

        private void SettingsProvider_SettingsChanged(object? sender, SettingsChangedEventArgs<RubberduckSettings> e)
        {
            _lsp.Workspace.DidChangeConfiguration(new() { Settings = Newtonsoft.Json.Linq.JToken.FromObject(e.NewValue) });
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            _service.ShowDialog();
            await Task.CompletedTask;
        }
    }
}
