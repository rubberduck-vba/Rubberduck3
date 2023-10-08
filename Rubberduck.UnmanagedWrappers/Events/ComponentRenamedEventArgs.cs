using Rubberduck.Unmanaged.Abstract;
using Rubberduck.Unmanaged.Model.Abstract;

namespace Rubberduck.Unmanaged.Events
{
    public class ComponentRenamedEventArgs : ComponentEventArgs
    {
        public ComponentRenamedEventArgs(IQualifiedModuleName qmn, string oldName)
            : base(qmn)
        {
            OldName = oldName;
        }

        public string OldName { get; }
    }
}