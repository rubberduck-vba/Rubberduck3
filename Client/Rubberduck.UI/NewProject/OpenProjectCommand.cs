using Rubberduck.UI.Command;
using Rubberduck.UI.Services;
using System;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Rubberduck.UI.NewProject
{
    public class OpenProjectCommand : CommandBase
    {
        private readonly IFileSystem _fileSystem;
        private readonly IProjectFileService _projectFileService;

        public OpenProjectCommand(UIServiceHelper service, 
            IProjectFileService projectFileService,
            IFileSystem fileSystem
            )
            : base(service)
        {
            _projectFileService = projectFileService;
            _fileSystem = fileSystem;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            string? uri;
            if (parameter is null)
            {
                var prompt = new BrowseFileModel
                {
                    Title = "Open Project",
                    DefaultFileExtension = "rdproj",
                    Filter = "Rubberduck Project (.rdproj);*.rdproj",
                };
                if (!DialogCommands.BrowseFileOpen(prompt))
                {
                    throw new OperationCanceledException();
                }
                uri = _fileSystem.Path.GetDirectoryName(prompt.Selection);
            }
            else
            {
                uri = parameter.ToString();
            }

            if (uri != null)
            {
                var workspaceUri = new Uri(uri);
                var model = _projectFileService.ReadFile(workspaceUri);
                // TODO load the workspace into the editor
            }
        }
    }
}
