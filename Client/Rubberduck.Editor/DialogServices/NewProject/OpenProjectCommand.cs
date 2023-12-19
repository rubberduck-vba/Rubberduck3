using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.UI.Command;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using System;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace Rubberduck.Editor.DialogServices.NewProject
{
    public class OpenProjectCommand : CommandBase
    {
        private readonly IFileSystem _fileSystem;
        private readonly IWorkspaceService _workspaceService;

        public OpenProjectCommand(UIServiceHelper service,
            IFileSystem fileSystem, IWorkspaceService workspace)
            : base(service)
        {
            _fileSystem = fileSystem;
            _workspaceService = workspace;
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
                    Filter = "Rubberduck Project (.rdproj)|*.rdproj",
                    RootUri = new Uri(_fileSystem.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Rubberduck", "Workspaces")),
                };
                if (!DialogCommands.BrowseFileOpen(prompt))
                {
                    return;
                }
                uri = _fileSystem.Path.GetDirectoryName(prompt.Selection);
            }
            else
            {
                uri = parameter.ToString();
            }

            if (uri != null)
            {
                if (!_workspaceService.ProjectFiles.Any(project => project.Uri.LocalPath[..^(ProjectFile.FileName.Length + 1)] == uri))
                {
                    Service.LogInformation("Opening project workspace...", $"Workspace root: {uri}");
                    await _workspaceService.OpenProjectWorkspaceAsync(new Uri(uri));
                }
            }
        }
    }
}
