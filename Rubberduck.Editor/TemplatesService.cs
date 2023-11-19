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

        public IEnumerable<ProjectTemplate> GetProjectTemplates()
        {
            foreach(var templateFolder in _fileSystem.Directory.GetDirectories(ApplicationConstants.TEMPLATES_FOLDER_PATH))
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
                var templatePath = _fileSystem.Path.Combine(ApplicationConstants.TEMPLATES_FOLDER_PATH, template.Name);

                var projectPath = _fileSystem.Path.Combine(templatePath, ".rdproj");
                var rdproj = _fileSystem.File.ReadAllText(projectPath);

                var projectFile = JsonSerializer.Deserialize<ProjectFile>(rdproj);
                if (projectFile is null)
                {
                    result = ProjectTemplate.Default;
                }
                else
                {
                    result = template with
                    {
                        Rubberduck = projectFile.Rubberduck,
                        ProjectFile = projectFile,
                    };
                }
            }))
            {
                LogWarning("Resolution failed: could not load the specified project template.", $"Template name: '{template.Name}'");
            }

            return result;
        }
    }
}
