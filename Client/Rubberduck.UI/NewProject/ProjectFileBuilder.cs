using Rubberduck.InternalApi.Model;
using Rubberduck.SettingsProvider;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;

namespace Rubberduck.UI.NewProject
{
    public class ProjectFileBuilder
    {
        private readonly IFileSystem _fileSystem;
        private readonly RubberduckSettingsProvider _settings;

        private Uri? _uri = default;
        private string _name = "Project1";

        private readonly List<Uri> _files = new();
        private readonly List<ProjectFile.Module> _modules = new();
        private readonly List<ProjectFile.Reference> _references = new();

        public ProjectFileBuilder(IFileSystem fileSystem, RubberduckSettingsProvider settings)
        {
            _settings = settings;
            _fileSystem = fileSystem;
            _references.Add(ProjectFile.Reference.VisualBasicForApplications);
        }

        private Uri DefaultUri => new(_fileSystem.Path.Combine(
            _settings.Settings.LanguageClientSettings.DefaultWorkspaceRoot.LocalPath, _name));

        public ProjectFile Build() => new()
        {
            Rubberduck = "3.0",
            Uri = _uri ?? DefaultUri,
            VBProject = new()
            {
                Name = _name,
                Modules = _modules.ToArray(),
                OtherFiles = _files.ToArray(),
                References = _references.ToArray(),
            },
        };

        public ProjectFileBuilder WithModel(INewProjectWindowViewModel viewModel)
        {
            _uri = new Uri(viewModel.WorkspaceLocation);
            _name = viewModel.ProjectName;
            return this;
        }

        public ProjectFileBuilder WithTemplate(ProjectTemplate template)
        {
            _name = template.ProjectFile.VBProject.Name;
            _files.AddRange(template.ProjectFile.VBProject.OtherFiles);
            _modules.AddRange(template.ProjectFile.VBProject.Modules);
            _references.AddRange(template.ProjectFile.VBProject.References);
            return this;
        }

        public ProjectFileBuilder WithUri(Uri uri)
        {
            _uri = uri;
            return this;
        }

        public ProjectFileBuilder WithProjectName(string name)
        {
            _name = name;
            return this;
        }

        public ProjectFileBuilder WithModule(Uri uri, DocClassType? supertype = null)
        {
            var localPath = uri.LocalPath;
            var name = _fileSystem.Path.GetFileNameWithoutExtension(localPath);

            _modules.Add(new()
            {
                Name = name,
                Uri = uri,
                Super = supertype
            });

            return this;
        }

        public ProjectFileBuilder WithFile(Uri uri)
        {
            _files.Add(uri);
            return this;
        }

        public ProjectFileBuilder WithReference(Uri uri, string name)
        {
            _references.Add(new()
            {
                Uri = uri,
                Name = name,
            });

            return this;
        }

        public ProjectFileBuilder WithReference(Uri uri, string name, Uri typeLibInfoUri)
        {
            _references.Add(new()
            {
                Uri = uri,
                Name = name,
                TypeLibInfoUri = typeLibInfoUri
            });

            return this;
        }

        public ProjectFileBuilder WithReference(Uri uri, Guid guid)
        {
            _references.Add(new()
            {
                Uri = uri,
                Guid = guid,
            });

            return this;
        }
    }
}
