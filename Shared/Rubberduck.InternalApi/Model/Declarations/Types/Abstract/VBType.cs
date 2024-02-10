using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

public abstract record class VBType
{
    public VBType(Type? managedType, string name, bool isUserDefined = false, bool isHidden = false)
    {
        ManagedType = managedType;
        Name = name;
        IsUserDefined = isUserDefined;
        IsHidden = isHidden;
    }

    public Type? ManagedType { get; init; }

    /// <summary>
    /// The symbolic name of the type, as it is used in code.
    /// </summary>
    /// <remarks>
    /// For user module types, this should be determined by a <c>VB_Name</c> attribute.
    /// </remarks>
    public string Name { get; init; }

    public bool IsUserDefined { get; init; }
    public bool IsHidden { get; init; }

    /// <summary>
    /// If <c>true</c>, the type is bound at run-time (i.e. late binding)
    /// </summary>
    public virtual bool RuntimeBinding { get; } = false;

    /// <summary>
    /// Gets the default managed value for this data type.
    /// </summary>
    public abstract VBTypedValue DefaultValue { get; }


    /// <summary>
    /// Whether this type can be passed by value.
    /// </summary>
    public virtual bool CanPassByValue { get; } = true;

    /// <summary>
    /// Override in derived types to specify VBTypes that are safe to convert this type into.
    /// </summary>
    public virtual VBType[] ConvertsSafelyToTypes { get; } = [];
    public bool ConvertsSafelyToType(VBType type) => ConvertsSafelyToTypes.Contains(type);
}
