using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Command;
using Rubberduck.UI.Message;
using Rubberduck.Unmanaged.Abstract;
using Rubberduck.VBEditor.UI.OfficeMenus;
using System;
using System.Threading.Tasks;

namespace Rubberduck.Main.Commands.ShowRubberduckEditor
{
    class ShowRubberduckEditorCommand : ComCommandBase, IShowRubberduckEditorCommand
    {
        private readonly IEditorServerProcessService _service;
        private readonly IMessageService _message;

        public ShowRubberduckEditorCommand(ILogger<ShowRubberduckEditorCommand> logger, ISettingsProvider<RubberduckSettings> settingsProvider, IVbeEvents vbeEvents, 
            IEditorServerProcessService service, IMessageService message)
            : base(logger, settingsProvider, vbeEvents)
        {
            _service = service;
            _message = message;
        }

        public event EventHandler Executed = delegate { };
        private void OnExecuted()
        {
            Executed?.Invoke(this, EventArgs.Empty);
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            var exception = _service.ShowEditor();
            if (exception is not null)
            {
                // TODO add this resource key
                _message.ShowError("RubberduckEditorProcessStartFailed", exception);
            }
            else
            {
                OnExecuted();
            }
            await Task.CompletedTask;
        }
    }
}