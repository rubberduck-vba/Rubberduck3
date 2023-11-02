using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB.Enums;
using Rubberduck.Unmanaged.Model;
using System;

namespace Rubberduck.Unmanaged.Events
{
    public class ReferenceEventArgs : EventArgs
    {
        public ReferenceEventArgs(ReferenceInfo reference, ReferenceKind type)
        {
            Reference = reference;
            Type = type;
        }

        public ReferenceInfo Reference { get; }
        public ReferenceKind Type { get; }
    }
}