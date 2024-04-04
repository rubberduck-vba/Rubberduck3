using Path = System.IO.Path;
using System;
using Rubberduck.Unmanaged.Model.Abstract;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB;
using System.IO;

namespace Rubberduck.Unmanaged.Model
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
            WorkspaceUri = project.Uri;
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
                    WorkspaceUri = project.Uri;
                }
            }
        }

        /// <summary>
        /// Creates a QualifiedModuleName for a library reference.
        /// Do not use this overload for referenced user projects.
        /// </summary>
        public QualifiedModuleName(ReferenceInfo reference) :this(reference.Name, reference.FullPath, reference.Name) { }

        /// <summary>
        /// Creates a QualifiedModuleName for a built-in declaration.
        /// Do not use this overload for user declarations.
        /// </summary>
        public QualifiedModuleName(string projectName, string projectPath, string componentName, Uri workspaceUri = null)
        {
            if (workspaceUri is null)
            {
                var root = new DirectoryInfo(projectPath).Parent.FullName;
                workspaceUri = new Uri(root);
            }

            _projectName = projectName;
            ProjectPath = projectPath;
            WorkspaceUri = workspaceUri;
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
        public Uri WorkspaceUri { get; }

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
            return HashCode.Combine(WorkspaceUri, _componentName);
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
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other.WorkspaceUri == WorkspaceUri 
                && other.ComponentName == ComponentName;
        }
    }
}
