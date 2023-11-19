using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Model;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model.LanguageClient;
using Rubberduck.UI;
using Rubberduck.UI.Message;
using Rubberduck.UI.NewProject;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text.Json;

namespace Rubberduck.Editor
{
    public class TemplatesService : ServiceBase
    {
        private readonly IFileSystem _fileSystem;
        private readonly IMessageService _messages;

        public TemplatesService(ILogger logger, RubberduckSettingsProvider settingsProvider,
            IFileSystem fileSystem, IMessageService messages) 
            : base(logger, settingsProvider)
        {
            _fileSystem = fileSystem;
            _messages = messages;
        }

        public void DeleteTemplate(string name)
        {
            TryRunAction(() =>
            {
                var templatePath = _fileSystem.Path.Combine(Settings.GeneralSettings.TemplatesLocation.LocalPath, name);
                _fileSystem.Directory.Delete(templatePath, true);
                LogInformation("Deleted project template.", $"Template: {name}");
            });
        }

        public void SaveAsTemplate(ProjectFile projectFile)
        {
            TryRunAction(() =>
            {
                var name = projectFile.VBProject.Name;
                var templatePath = _fileSystem.Path.Combine(Settings.GeneralSettings.TemplatesLocation.LocalPath, name);

                if (_fileSystem.Directory.Exists(templatePath))
                {
                    if (!ConfirmOverwriteTemplate(name))
                    {
                        throw new OperationCanceledException();
                    }
                    DeleteTemplate(name);
                }

                _fileSystem.Directory.CreateDirectory(templatePath);
                
                var templateSourcePath = _fileSystem.Path.Combine(templatePath, ".template");
                _fileSystem.Directory.CreateDirectory(templateSourcePath);
                LogTrace($"Created .template folder.");

                var serializedProjectFile = JsonSerializer.Serialize(projectFile);
                var projectFilePath = _fileSystem.Path.Combine(templatePath, ".rdproj");
                _fileSystem.File.WriteAllText(projectFilePath, serializedProjectFile);
                LogTrace($"Serialized .rdproj file.");

                var modules = projectFile.VBProject.Modules.Length;
                foreach (var module in projectFile.VBProject.Modules.Select((e, i) => (Element: e, Index: i)))
                {
                    var originalPath = _fileSystem.Path.Combine(projectFile.Uri.LocalPath, ".src", module.Element.Uri.LocalPath);
                    var newPath = _fileSystem.Path.Combine(templateSourcePath, module.Element.Uri.LocalPath);
                    _fileSystem.File.Copy(originalPath, newPath, overwrite: true);
                    LogTrace($"Copied source file {module.Index + 1} of {modules}.", $"{module.Element.Uri}");
                }

                var files = projectFile.VBProject.OtherFiles.Length;
                foreach (var uri in projectFile.VBProject.OtherFiles.Select((e, i) => (Element: e, Index: i)))
                {
                    var originalPath = _fileSystem.Path.Combine(projectFile.Uri.LocalPath, ".src", uri.Element.LocalPath);
                    var newPath = _fileSystem.Path.Combine(templateSourcePath, uri.Element.LocalPath);
                    _fileSystem.File.Copy(originalPath, newPath, overwrite: true);
                    LogTrace($"Copied misc. file {uri.Index + 1} of {files}.", $"{uri.Element}");
                }

                LogInformation("New project template created.", $"Template: {name} ({modules + files} files, {modules} synchronized)");
            });
        }

        private bool ConfirmOverwriteTemplate(string name)
        {
            var model = new MessageRequestModel
            {
                Key = "ConfirmOverwriteProjectTemplate",
                Title = "Confirm Overwrite",
                Message = $"A project template named '{name}' already exists. Overwrite the existing template?",
                Level = LogLevel.Warning,
                MessageActions = [MessageAction.AcceptConfirmAction, MessageAction.CancelAction]
            };

            var result = _messages.ShowMessageRequest(model);
            if (result.MessageAction.IsDefaultAction)
            {
                if (!result.IsEnabled)
                {
                    DisabledMessageKeysSetting.DisableMessageKey(model.Key, SettingsProvider);
                }
                return true;
            }

            return false;
        }

        public IEnumerable<ProjectTemplate> GetProjectTemplates()
        {
            yield return ProjectTemplate.Default;

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
