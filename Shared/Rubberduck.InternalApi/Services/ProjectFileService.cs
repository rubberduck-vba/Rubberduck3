using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.InternalApi.Settings;
using System;
using System.IO.Abstractions;
using System.Text.Json;

namespace Rubberduck.InternalApi.Services
{
    public class ProjectFileService : ServiceBase, IProjectFileService
    {
        private readonly IFileSystem _fileSystem;

        public ProjectFileService(ILogger<ProjectFileService> logger, RubberduckSettingsProvider settingsProvider,
            IFileSystem fileSystem, PerformanceRecordAggregator performance)
            : base(logger, settingsProvider, performance)
        {
            _fileSystem = fileSystem;
        }

        public void CreateFile(ProjectFile model)
        {
            var path = _fileSystem.Path.Combine(model.Uri.LocalPath, ProjectFile.FileName);
            var content = JsonSerializer.Serialize(model);

            _fileSystem.File.WriteAllText(path, content);
        }

        public ProjectFile ReadFile(Uri root)
        {
            var path = _fileSystem.Path.Combine(root.LocalPath, ProjectFile.FileName);
            var content = _fileSystem.File.ReadAllText(path);
            var projectFile = JsonSerializer.Deserialize<ProjectFile>(content) ?? throw new InvalidOperationException();

            projectFile.Uri = root;
            return projectFile;
        }
    }
}
