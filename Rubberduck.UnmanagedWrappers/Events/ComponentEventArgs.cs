using Rubberduck.Unmanaged.Abstract;
using Rubberduck.Unmanaged.Model.Abstract;
using System;

namespace Rubberduck.Unmanaged.Events
{
    public class ComponentEventArgs : EventArgs
    {
        public ComponentEventArgs(IQualifiedModuleName qualifiedModuleName)
        {
            QualifiedModuleName = qualifiedModuleName;
        }

        public string ProjectId => QualifiedModuleName.ProjectId;
        public IQualifiedModuleName QualifiedModuleName { get; }
    }
}