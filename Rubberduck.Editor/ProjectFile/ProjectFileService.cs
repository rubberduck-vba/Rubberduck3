using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB;
using Rubberduck.Unmanaged.TypeLibs.Abstract;
using Rubberduck.Unmanaged.TypeLibs.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Text.Json;
using static Rubberduck.Editor.ProjectFile.ProjectFile;

namespace Rubberduck.Editor.ProjectFile
{

    public class ProjectFileService
    {
        private const string _extension = ".rdproj";

        private readonly IFileSystem _fileSystem;
        private readonly ISettingsProvider<RubberduckSettings> _settings;
        private readonly IVBETypeLibsAPI _api;
        private readonly IVBE _vbe;

        public ProjectFileService(IVBE vbe, IVBETypeLibsAPI api, IFileSystem fileSystem, ISettingsProvider<RubberduckSettings> settings)
        {
            _api = api;
            _vbe = vbe;
            _fileSystem = fileSystem;
            _settings = settings;
        }

        public void CreateFile(ProjectFile model)
        {
            var content = JsonSerializer.Serialize(model);
            var root = model.Uri.LocalPath;
            var path = _fileSystem.Path.Combine(root, $"/{_extension}");
            if (!_fileSystem.Directory.Exists(root))
            {
                _fileSystem.Directory.CreateDirectory(root);
            }

            _fileSystem.File.WriteAllText(path, content);
        }

        public ProjectFile ReadFile(Uri root)
        {
            var path = _fileSystem.Path.Combine(root.LocalPath, $"/{_extension}");
            if (_fileSystem.File.Exists(path))
            {
                var content = _fileSystem.File.ReadAllText(path);
                return JsonSerializer.Deserialize<ProjectFile>(content) ?? throw new InvalidOperationException();
            }

            throw new FileNotFoundException("No project file was found at the specified workspace root uri.");
        }

        public Project FromVBProject(IVBProject project, string srcRoot)
        {
            var modules = ExportModules(project, srcRoot);
            return new Project
            {
                Name = project.Name,
                References = GetReferences(project),
                Modules = modules,
            };
        }

        private Reference[] GetReferences(IVBProject project)
        {
            var result = new List<Reference>();
            using var references = project.References;
            for (var priorityId = 0; priorityId < references.Count - 1; priorityId++)
            {
                var reference = references[priorityId];
                if (!reference.IsBuiltIn)
                {
                    // user code is referenced. if locked, export the typelib info.
                    // if unlocked, we'll want to load a second workspace for that project.

                    // for non-user code references, include a URI to a file containing the serialized typelib info.
                    // there should be a workspace-independent folder for these files.

                    // if typelib info is disabled, server uses the provided Uri or Guid to load the library and extract the type info.
                }

                var apiReference = _api.GetReferenceInfo(project, reference);
                var uri = new Uri(apiReference.Path);

                result.Add(new Reference
                {
                    Name = reference.Name,
                    Uri = uri,
                    Guid = !string.IsNullOrWhiteSpace(reference.Guid) ? Guid.Parse(reference.Guid) : Guid.Empty,
                    TypeLibInfoUri = null,
                });
            }
            return result.ToArray();
        }

        private Module[] ExportModules(IVBProject project, string srcRoot)
        {
            var result = new List<Module>();
            using var components = project.VBComponents;
            foreach (var component in components)
            {
                DocClassType? superType = null;
                if (component.Type == Unmanaged.Model.ComponentType.Document)
                {
                    superType = _api.DetermineDocumentClassType(_vbe, project.Name, component.Name);
                }

                var file = component.ExportAsSourceFile(srcRoot);
                if (!string.IsNullOrWhiteSpace(file))
                {
                    var module = new Module
                    {
                        Name = component.Name,
                        Super = superType,
                        Uri = new Uri(file),
                    };
                    result.Add(module);
                }
            }

            return result.ToArray();
        }

    }
}
