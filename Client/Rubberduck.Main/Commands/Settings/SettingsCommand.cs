using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using Rubberduck.UI.Command;
using Rubberduck.UI.Services;
using Rubberduck.UI.Settings;
using Rubberduck.Unmanaged.Abstract;
using Rubberduck.VBEditor.UI.OfficeMenus;
using System.Threading.Tasks;

namespace Rubberduck.Main.Settings
{
    public class SettingsCommand : ComCommandBase, ISettingsCommand
    {
        private readonly ISettingsDialogService _service;
        //private readonly ILanguageClientFacade _lsp;

        public SettingsCommand(UIServiceHelper service, IVbeEvents vbeEvents, 
            //ILanguageClientFacade lsp,
            ISettingsDialogService dialogService)
            : base(service, vbeEvents)
        {
            _service = dialogService;
            //_lsp = lsp;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            _service.ShowDialog();
            //_lsp.Workspace.DidChangeConfiguration(new() { Settings = Newtonsoft.Json.Linq.JToken.FromObject(e.NewValue) });
            await Task.CompletedTask;
        }
    }
}
