using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using System.Threading.Tasks;

namespace Rubberduck.Editor.DialogServices.NewProject
{
    public class SaveAsProjectTemplateCommand : CommandBase
    {
        private readonly IWorkspaceService _workspace;
        private readonly ITemplatesService _templates;

        public SaveAsProjectTemplateCommand(UIServiceHelper service,
            IWorkspaceService workspace, ITemplatesService templates)
            : base(service)
        {
            _workspace = workspace;
            _templates = templates;
        }

        protected override async Task OnExecuteAsync(object? parameter)
        {
            var template = new ProjectTemplate(); // TODO ExportTemplateService
            _templates.SaveProjectTemplate(template);
        }
    }
}
