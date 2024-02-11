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

        public Uri WorkspaceUri => QualifiedModuleName.WorkspaceUri;
        public IQualifiedModuleName QualifiedModuleName { get; }
    }
}