using System.Globalization;
using Path = System.IO.Path;
using Rubberduck.VBEditor.SafeComWrappers.Abstract;
using System;
using Rubberduck.VBEditor.Extensions;
using Rubberduck.InternalApi.Model;

namespace Rubberduck.VBEditor
{
    /// <summary>
    /// Represents a VBComponent or a VBProject.
    /// </summary>
    public class QualifiedModuleName : IQualifiedModuleName
    {
        public static QualifiedModuleName None { get; } = new QualifiedModuleName();

        public QualifiedModuleName(IVBProject project)
        {
            _componentName = null;
            ComponentType = ComponentType.Undefined;
            _projectName = project.Name;
            ProjectPath = string.Empty;
            ProjectId = project.GetProjectId();
        }

        public QualifiedModuleName(IVBComponent component)
        {
            ComponentType = component.Type;
            _componentName = component.IsWrappingNullReference ? string.Empty : component.Name;

            using (var components = component.Collection)
            {
                using (var project = components.Parent)
                {
                    _projectName = project == null ? string.Empty : project.Name;
                    ProjectPath = string.Empty;
                    ProjectId = project.GetProjectId();
                }
            }
        }

        /// <summary>
        /// Creates a QualifiedModuleName for a library reference.
        /// Do not use this overload for referenced user projects.
        /// </summary>
        public QualifiedModuleName(ReferenceInfo reference)
        :this(reference.Name, reference.FullPath, reference.Name) { }

        /// <summary>
        /// Creates a QualifiedModuleName for a built-in declaration.
        /// Do not use this overload for user declarations.
        /// </summary>
        public QualifiedModuleName(string projectName, string projectPath, string componentName, string projectId = null)
        {
            _projectName = projectName;
            ProjectPath = projectPath;
            ProjectId = projectId ?? $"External {_projectName};{ProjectPath}".GetHashCode().ToString(CultureInfo.InvariantCulture);
            _componentName = componentName;
            ComponentType = ComponentType.ComComponent;
        }

        private QualifiedModuleName()
        {
        }

        public QualifiedMemberName QualifyMemberName(string member)
        {
            return new QualifiedMemberName(this, member);
        }

        public ComponentType ComponentType { get; }

        public bool IsParsable => ComponentType != ComponentType.ResFile && ComponentType != ComponentType.RelatedDocument;
        public string ProjectId { get; }

        private readonly string _componentName;
        public string ComponentName => _componentName ?? string.Empty;

        public string Name => ToString();

        private readonly string _projectName;
        public string ProjectName => _projectName ?? string.Empty;

        public string ProjectPath { get; }

        public override string ToString()
        {
            return string.IsNullOrEmpty(_componentName) && string.IsNullOrEmpty(_projectName)
                ? string.Empty
                : (string.IsNullOrEmpty(ProjectPath) ? string.Empty : Path.GetFileName(ProjectPath) + ";")
                     + $"{_projectName}.{_componentName}";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ProjectId ?? string.Empty, _componentName ?? string.Empty);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            return Equals(obj as IQualifiedModuleName);
        }

        public bool Equals(IQualifiedModuleName other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other.ProjectId == ProjectId 
                && other.ComponentName == ComponentName;
        }
    }

    public static class QualifiedModuleNameExtensions
    {
        public static bool TryGetProject(this QualifiedModuleName moduleName, IVBE vbe, out IVBProject project)
        {
            using (var projects = vbe.VBProjects)
            {
                foreach (var item in projects)
                {
                    if (item.ProjectId == moduleName.ProjectId && item.Name == moduleName.ProjectName)
                    {
                        project = item;
                        return true;
                    }

                    item.Dispose();
                }

                project = null;
                return false;
            }
        }

        public static bool TryGetComponent(this QualifiedModuleName moduleName, IVBE vbe, out IVBComponent component)
        {
            if (TryGetProject(moduleName, vbe, out var project))
            {
                using (project)
                using (var components = project.VBComponents)
                {
                    component = components[moduleName.ComponentName];
                    return true;
                }
            }

            component = null;
            return false;
        }
    }
}
