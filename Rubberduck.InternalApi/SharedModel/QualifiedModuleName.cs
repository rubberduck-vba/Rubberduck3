using System;
using System.IO;

namespace Rubberduck.InternalApi.Model
{
    /// <summary>
    /// Represents a VBComponent or a VBProject.
    /// </summary>
    public interface IQualifiedModuleName : IEquatable<IQualifiedModuleName>
    {
        ComponentType ComponentType { get; }
        string ProjectId { get; }
        bool IsParsable { get; }
        string Name { get; }
        string ComponentName { get; }
        string ProjectName { get; }
        string ProjectPath { get; }
    }


    public class SerializableQualifiedModuleName : IQualifiedModuleName
    {
        public ComponentType ComponentType { get; set; }

        public bool IsParsable => ComponentType != ComponentType.ResFile && ComponentType != ComponentType.RelatedDocument;
        public string ProjectId { get; set; }
        public string ComponentName { get; set; }

        public string Name => ToString();

        public string ProjectName { get; set; }

        public string ProjectPath { get; set; }

        public override string ToString()
        {
            return string.IsNullOrEmpty(ComponentName) && string.IsNullOrEmpty(ProjectName)
                ? string.Empty
                : (string.IsNullOrEmpty(ProjectPath) ? string.Empty : Path.GetFileName(ProjectPath) + ";")
                     + $"{ProjectName}.{ComponentName}";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ProjectId ?? string.Empty, ComponentName ?? string.Empty);
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

            return other?.ProjectId == ProjectId 
                && other?.ComponentName == ComponentName;
        }
    }
}
