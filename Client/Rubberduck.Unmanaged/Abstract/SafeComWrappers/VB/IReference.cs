﻿using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB.Enums;
using System;

namespace Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB
{
    public interface IReference : ISafeComWrapper, IEquatable<IReference>
    {
        string Name { get; }
        string Guid { get; }
        string Description { get; }
        int Major { get; }
        int Minor { get; }
        string Version { get; }
        string FullPath { get; }
        bool IsBuiltIn { get; }
        bool IsBroken { get; }
        ReferenceKind Type { get; }
        IReferences Collection { get; }
        IVBE VBE { get; }
    }
}
