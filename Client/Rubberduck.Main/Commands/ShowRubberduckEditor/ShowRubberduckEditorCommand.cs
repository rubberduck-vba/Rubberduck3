using Rubberduck.Main.RPC.EditorServer;
using Rubberduck.UI.Command;
using Rubberduck.UI.Message;
using Rubberduck.UI.Services;
using Rubberduck.Unmanaged.Abstract;
using Rubberduck.VBEditor.UI.OfficeMenus;
using System.Threading.Tasks;

namespace Rubberduck.Main.Commands.ShowRubberduckEditor
{
    class ShowRubberduckEditorCommand : ComCommandBase, IShowRubberduckEditorCommand
    {
        private readonly IEditorServerProcessService _processService;
        private readonly IMessageService _message;
        private readonly EditorClientApp _client;

        public ShowRubberduckEditorCommand(ServiceHelper service, IVbeEvents vbeEvents, 
            IEditorServerProcessService process,
            EditorClientApp clientApp,
            IMessageService message)
            : base(service, vbeEvents)
        {
            _processService = process;
            _message = message;
            _client = clientApp;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            if (_processService.StartEditorProcess())
            {
                Service.LogTrace("Editor server process was started; starting addin LSP client...");
                await _client.StartupAsync();
            }
            await Task.CompletedTask;
        }
    }
}