using Rubberduck.UI.Command;
using Rubberduck.UI.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Rubberduck.UI.NewProject
{
    public class NewProjectCommand : CommandBase
    {
        private readonly NewProjectWindowFactory _factory;
        private readonly MessageActionsProvider _actions;
        private readonly ShowRubberduckSettingsCommand _showSettingsCommand;

        public NewProjectCommand(ServiceHelper service, NewProjectWindowFactory factory, 
            MessageActionsProvider actions, ShowRubberduckSettingsCommand showSettingsCommand) 
            : base(service)
        {
            _factory = factory;
            _actions = actions;
            _showSettingsCommand = showSettingsCommand;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            Service.TryRunAction(() =>
            {
                // TODO get VBProjectInfo items from VBE addin client
                var model = new NewProjectWindowViewModel(Service.Settings, Enumerable.Empty<VBProjectInfo?>(), _actions, _showSettingsCommand);
                var view = _factory.Create(model);
                if (view.ShowDialog() == true)
                {
                    // TODO save .rdproj project file in workspace root
                }
            }, nameof(NewProjectCommand));

            await Task.CompletedTask;
        }
    }
}
