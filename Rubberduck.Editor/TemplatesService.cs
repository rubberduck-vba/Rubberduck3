using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Model;
using Rubberduck.Resources;
using Rubberduck.SettingsProvider;
using Rubberduck.UI.NewProject;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rubberduck.Editor
{
    public class TemplatesService : ServiceBase
    {
        private readonly IFileSystem _fileSystem;

        public TemplatesService(ILogger logger, RubberduckSettingsProvider settingsProvider,
            IFileSystem fileSystem) 
            : base(logger, settingsProvider)
        {
            _fileSystem = fileSystem;
        }

        public void SaveAsTemplate(ProjectFile projectFile)
        {
            TryRunAction(() =>
            {
                var name = projectFile.VBProject.Name;
                var templatePath = _fileSystem.Path.Combine(Settings.GeneralSettings.TemplatesLocation.LocalPath, name);


            });
        }

        public IEnumerable<ProjectTemplate> GetProjectTemplates()
        {
            foreach(var templateFolder in _fileSystem.Directory.GetDirectories(Settings.GeneralSettings.TemplatesLocation.LocalPath))
            {
                var name = templateFolder;
                yield return new ProjectTemplate
                {
                    Name = name,
                };
            }
        }

        public ProjectTemplate Resolve(ProjectTemplate template)
        {
            var result = template;
            if (!TryRunAction(() =>
            {
                var projectPath = _fileSystem.Path.Combine(template.Name, ".rdproj");
                var content = _fileSystem.File.ReadAllText(projectPath);

                var projectFile = JsonSerializer.Deserialize<ProjectFile>(content);
                if (projectFile is null)
                {
                    LogWarning("Failed to deserialize project file; using default project template.", $"Content: \n{content}");
                    result = ProjectTemplate.Default;
                }
                else
                {
                    result = template with
                    {
                        Rubberduck = projectFile.Rubberduck,
                        ProjectFile = projectFile,
                    };
                    LogInformation("Resolved template project file.", $"Template: {template.Name}");
                }
            }))
            {
                LogWarning("Resolution failed: could not load the specified project template.", $"Template name: '{template.Name}'");
            }

            return result;
        }
    }
}
