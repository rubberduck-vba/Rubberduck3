using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Model;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model.LanguageClient;
using Rubberduck.UI;
using Rubberduck.UI.Message;
using Rubberduck.UI.NewProject;
using Rubberduck.UI.Services.Abstract;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text.Json;

namespace Rubberduck.Editor
{
    public class TemplatesService : ServiceBase, ITemplatesService
    {
        private readonly IFileSystem _fileSystem;
        private readonly IMessageService _messages;
        
        public TemplatesService(ILogger<TemplatesService> logger, RubberduckSettingsProvider settingsProvider,
            IFileSystem fileSystem, IMessageService messages, PerformanceRecordAggregator performance) 
            : base(logger, settingsProvider, performance)
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
            if (!TryRunAction(() =>
            {
                var name = projectFile.VBProject.Name;

                var templateRoot = CreateTemplateRoot(name);
                CreateProjectFile(projectFile, templateRoot);

                var sourceRoot = CreateTemplateSourceFolder(templateRoot, projectFile.VBProject.Folders);
                
                var (files, bytes) = CopyProjectFiles(projectFile, sourceRoot);
                if (bytes == 0 && files != 0)
                {
                    LogWarning($"Template was created for {files}, totalling 0 bytes.");
                }

                LogInformation("New project template created.", 
                    $"Template: {name}; {files} files ({bytes} bytes); {projectFile.VBProject.Folders.Length} folders.");

            }, out var exception) && exception is not null)
            {
                throw new TimedActionFailedException(exception);
            }
        }

        private (int, long) CopyProjectFiles(ProjectFile projectFile, string sourceRoot)
        {
            var files = projectFile.VBProject.OtherFiles.Concat(projectFile.VBProject.Modules).ToList().Count;
            var filesCopied = 0;
            var bytesCopied = 0L;

            if (TryRunAction(() =>
            {
                foreach (var indexedFile in projectFile.VBProject.OtherFiles.Select((e, i) => (File: e, Index: i)))
                {
                    if (!TryRunAction(() =>
                    {
                        var originalPath = _fileSystem.Path.Combine(projectFile.Uri.LocalPath, ProjectFile.SourceRoot, indexedFile.File.Uri);
                        var fileLength = _fileSystem.FileInfo.New(originalPath).Length;

                        var newPath = _fileSystem.Path.Combine(sourceRoot, indexedFile.File.Uri);
                        _fileSystem.Directory.CreateDirectory(newPath);
                        _fileSystem.File.Copy(originalPath, newPath, overwrite: true);

                        filesCopied++;
                        bytesCopied += fileLength;
                        LogTrace($"Copied file {indexedFile.Index + 1} of {files}.", $"File: '{indexedFile.File}' ({(indexedFile.File is Module ? "code" : "misc.")}, {fileLength} bytes)");
                    }, out var exception))
                    {
                        if (exception is not null)
                        {
                            LogException(exception, $"Ignoring file: '{indexedFile.File.Uri}'");
                        }
                    }
                }
            }) && filesCopied > 0)
            {
                LogInformation("Project template miscellaneous files created.", $"Path: {sourceRoot}; {filesCopied} files ({bytesCopied} bytes)");
            }

            return (filesCopied, bytesCopied);
        }

        private void CreateProjectFile(ProjectFile projectFile, string templateRoot)
        {
            if (!TryRunAction(() =>
            {
                var serializedProjectFile = JsonSerializer.Serialize(projectFile);
                var projectFilePath = _fileSystem.Path.Combine(templateRoot, ProjectFile.FileName);
                _fileSystem.File.WriteAllText(projectFilePath, serializedProjectFile);
                LogTrace($"Serialized .rdproj file.", $"Path: {projectFilePath}");
            }, out var exception))
            {
                if (exception is not null)
                {
                    throw new TimedActionFailedException(exception);
                }
            }
        }

        private string CreateTemplateRoot(string name)
        {
            var path = _fileSystem.Path.Combine(Settings.GeneralSettings.TemplatesLocation.LocalPath, name);

            if (!TryRunAction(() =>
            {
                if (_fileSystem.Directory.Exists(path))
                {
                    if (!ConfirmOverwriteTemplate(name))
                    {
                        throw new OperationCanceledException();
                    }
                    DeleteTemplate(name);
                }

                _fileSystem.Directory.CreateDirectory(path);
                LogTrace($"Created template root folder.", $"Path: {path}");
            }, out var exception))
            {
                if (exception is not null)
                {
                    throw new TimedActionFailedException(exception);
                }
            }

            return path;
        }

        private string CreateTemplateSourceFolder(string root, IEnumerable<Folder> folders)
        {
            var path = _fileSystem.Path.Combine(root, ProjectTemplate.TemplateSourceFolderName);
            if (!TryRunAction(() =>
            {
                _fileSystem.Directory.CreateDirectory(path);
                LogTrace($"Created .template source folder.", $"Path: {path}");

                foreach (var folder in folders)
                {
                    _fileSystem.Directory.CreateDirectory(folder.Uri);
                    LogTrace($"Created folder: {folder.Uri}");
                }
            }, out var exception))
            {
                if (exception is not null)
                {
                    throw new TimedActionFailedException(exception);
                }
            }
            return path;
        }

        private bool ConfirmOverwriteTemplate(string name)
        {
            var model = new MessageRequestModel
            {
                Key = "ConfirmOverwriteProjectTemplate",
                Title = "Confirm Overwrite",
                Message = $"A project template named '{name}' already exists. Overwrite the existing template? This will DELETE the entire subdirectory and cannot be undone.",
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
                var name = _fileSystem.Path.GetFileName(templateFolder)!;
                var templateSourceFolder = _fileSystem.Path.Combine(templateFolder, ProjectTemplate.TemplateSourceFolderName);
                if (!_fileSystem.Directory.Exists(templateSourceFolder))
                {
                    LogWarning($"Template folder '{name}' is not a valid project template.", $"Template source folder '{ProjectTemplate.TemplateSourceFolderName}' was not found.");
                    continue;
                }

                var projectFilePath = _fileSystem.Path.Combine(templateFolder, ProjectFile.FileName);
                if (!_fileSystem.File.Exists(projectFilePath))
                {
                    LogWarning($"Template folder '{name}' is not a valid project template.", $"Project file '{ProjectFile.FileName}' was not found.");
                    continue;
                }

                // return minimally hydrated objects; Resolve(ProjectTemplate) does the rest.
                LogInformation($"Found project template: {name}");
                yield return new ProjectTemplate { Name = name };
            }
        }

        public ProjectTemplate Resolve(ProjectTemplate template)
        {
            var result = template;
            if (!TryRunAction(() =>
            {
                var root = Settings.GeneralSettings.TemplatesLocation.LocalPath;
                var projectPath = _fileSystem.Path.Combine(root, template.Name, ProjectFile.FileName);
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
                        ProjectFile = projectFile
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
