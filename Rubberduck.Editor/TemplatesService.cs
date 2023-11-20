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
    public class TemplatesService : ServiceBase, ITemplatesService
    {
        private readonly IFileSystem _fileSystem;
        private readonly IMessageService _messages;
        
        public TemplatesService(ILogger<TemplatesService> logger, RubberduckSettingsProvider settingsProvider,
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
            if (!TryRunAction(() =>
            {
                var name = projectFile.VBProject.Name;

                var templateRoot = CreateTemplateRoot(name);
                CreateProjectFile(projectFile, templateRoot);

                var sourceRoot = CreateTemplateSourceFolder(templateRoot);
                
                var (sourceFiles, sourceBytes) = CopyProjectSourceFiles(projectFile, sourceRoot);
                var (otherFiles, otherBytes) = CopyProjectOtherFiles(projectFile, sourceRoot);
                
                var templateFiles = sourceFiles + otherFiles;
                var templateBytes = sourceBytes + otherBytes;

                if (templateBytes == 0 && templateFiles != 0)
                {
                    LogWarning($"Template was created for {templateFiles}, totalling {templateBytes} bytes.");
                }

                LogInformation("New project template created.", 
                    $"Template: {name}; {templateFiles} files, {templateBytes} bytes; source files: {sourceFiles} files, {sourceBytes} bytes; other: {otherFiles} files, {otherBytes} bytes.");

            }, out var exception) && exception is not null)
            {
                throw new TimedActionFailedException(exception);
            }
        }

        private (int, long) CopyProjectOtherFiles(ProjectFile projectFile, string sourceRoot)
        {
            var files = projectFile.VBProject.OtherFiles.Length;
            var filesCopied = 0;
            var bytesCopied = 0L;

            if (TryRunAction(() =>
            {
                foreach (var uri in projectFile.VBProject.OtherFiles.Select((e, i) => (Element: e, Index: i)))
                {
                    if (!TryRunAction(() =>
                    {
                        var originalPath = _fileSystem.Path.Combine(projectFile.Uri.LocalPath, ProjectFile.SourceRoot, uri.Element.LocalPath);
                        var fileLength = _fileSystem.FileInfo.New(originalPath).Length;

                        var newPath = _fileSystem.Path.Combine(sourceRoot, uri.Element.LocalPath);

                        _fileSystem.File.Copy(originalPath, newPath, overwrite: true);

                        filesCopied++;
                        bytesCopied += fileLength;
                        LogTrace($"Copied misc. file {uri.Index + 1} of {files}.", $"File: '{uri.Element}' ({fileLength} bytes)");
                    }, out var exception))
                    {
                        if (exception is not null)
                        {
                            LogException(exception, $"Ignoring file: '{uri.Element}'");
                        }
                    }
                }
            }) && filesCopied > 0)
            {
                LogInformation("Project template miscellaneous files created.", $"Path: {sourceRoot}; {filesCopied} files ({bytesCopied} bytes)");
            }

            return (filesCopied, bytesCopied);
        }

        private (int, long) CopyProjectSourceFiles(ProjectFile projectFile, string sourceRoot)
        {
            var modules = projectFile.VBProject.Modules.Length;
            var filesCopied = 0;
            var bytesCopied = 0L;

            if (TryRunAction(() =>
            {
                foreach (var module in projectFile.VBProject.Modules.Select((e, i) => (Element: e, Index: i)))
                {
                    if (!TryRunAction(() =>
                    {
                        var originalPath = _fileSystem.Path.Combine(projectFile.Uri.LocalPath, ProjectFile.SourceRoot, module.Element.Uri.LocalPath);
                        var fileLength = _fileSystem.FileInfo.New(originalPath).Length;

                        var newPath = _fileSystem.Path.Combine(sourceRoot, module.Element.Uri.LocalPath);

                        _fileSystem.File.Copy(originalPath, newPath, overwrite: true);

                        filesCopied++;
                        bytesCopied += fileLength;
                        LogTrace($"Copied source file {module.Index + 1} of {modules}.", $"File: '{module.Element.Uri}' ({fileLength} bytes)");
                    }, out var exception))
                    {
                        if (exception is not null)
                        {
                            LogException(exception, $"Ignoring source file: '{module.Element.Uri}'");
                        }
                    }
                }
            }) && filesCopied > 0)
            {
                LogInformation("Project template source files created.", $"Path: {sourceRoot}; {filesCopied} files ({bytesCopied} bytes)");
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

        private string CreateTemplateSourceFolder(string root)
        {
            var path = _fileSystem.Path.Combine(root, ProjectTemplate.TemplateSourceFolderName);
            if (!TryRunAction(() =>
            {
                _fileSystem.Directory.CreateDirectory(path);
                LogTrace($"Created .template source folder.", $"Path: {path}");
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
                var name = _fileSystem.Path.GetDirectoryName(templateFolder)!;
                var templateSourceFolder = _fileSystem.Path.Combine(templateFolder, ProjectTemplate.TemplateSourceFolderName);
                if (!_fileSystem.Directory.Exists(templateSourceFolder))
                {
                    LogWarning($"Template folder '{name}' is not a valid project template.", $"Template source folder '{ProjectTemplate.TemplateSourceFolderName}' was not found.");
                    continue;
                }

                var projectFilePath = _fileSystem.Path.Combine(templateSourceFolder, ProjectFile.FileName);
                if (!_fileSystem.File.Exists(projectFilePath))
                {
                    LogWarning($"Template folder '{name}' is not a valid project template.", $"Project file '{ProjectFile.FileName}' was not found.");
                    continue;
                }

                // return minimally hydrated objects; Resolve(ProjectTemplate) does the rest.
                yield return new ProjectTemplate { Name = name };
            }
        }

        public ProjectTemplate Resolve(ProjectTemplate template)
        {
            var result = template;
            if (!TryRunAction(() =>
            {
                var projectPath = _fileSystem.Path.Combine(template.Name, ProjectFile.FileName);
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
