using System;

namespace Rubberduck.Unmanaged.Model.Abstract
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
}
