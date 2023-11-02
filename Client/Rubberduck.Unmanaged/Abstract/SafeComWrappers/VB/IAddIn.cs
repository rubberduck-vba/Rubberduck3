using System;
using System.Collections.Generic;

namespace Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB
{
    public interface IAddIn : ISafeComWrapper, IEquatable<IAddIn>
    {
        string ProgId { get; }
        string Guid { get; }
        string Description { get; set; }
        bool Connect { get; set; }
        object Object { get; set; }

        IVBE VBE { get; }
        IAddIns Collection { get; }
        IReadOnlyDictionary<CommandBarSite, CommandBarLocation> CommandBarLocations { get; }
    }
}