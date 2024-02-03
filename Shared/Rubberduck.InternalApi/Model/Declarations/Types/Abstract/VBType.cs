using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using System;
using System.Collections.Immutable;

namespace Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

public abstract record class VBType<TValue> : VBType
{
    protected VBType(string name, bool isUserDefined = false, bool isHidden = false)
        : base(typeof(TValue), name, isUserDefined, isHidden)
    {
    }

    /// <summary>
    /// If <c>true</c>, the type is bound at run-time (i.e. late binding)
    /// </summary>
    public virtual bool RuntimeBinding { get; } = false;

    /// <summary>
    /// Gets the default managed value for this data type.
    /// </summary>
    public abstract TValue? DefaultValue { get; }
}

/// <summary>
/// A metatype that describes a type. Not used in many places!
/// </summary>
public record class VBTypeDescValue : VBTypedValue
{
    public VBTypeDescValue(VBType type) : base(type)
    {
    }
}

public abstract record class VBType
{
    #region intrinsic types
    public static VBType VbBooleanType { get; } = VBBooleanType.TypeInfo;
    public static VBType VbByteType { get; } = VBByteType.TypeInfo;
    public static VBType VbIntegerType { get; } = VBIntegerType.TypeInfo;
    public static VBType VbLongType { get; } = VBLongType.TypeInfo;
    public static VBType VbLongPtrType { get; } = VBLongPtrType.TypeInfo;
    public static VBType VbLongLongType { get; } = VBLongLongType.TypeInfo;
    public static VBType VbCurrencyType { get; } = VBCurrencyType.TypeInfo;
    public static VBType VbDecimalType { get; } = VBDecimalType.TypeInfo;
    public static VBType VbSingleType { get; } = VBSingleType.TypeInfo;
    public static VBType VbDoubleType { get; } = VBDoubleType.TypeInfo;
    public static VBType VbDateType { get; } = VBDateType.TypeInfo;
    public static VBType VbStringType { get; } = VBStringType.TypeInfo;
    public static VBType VbObjectType { get; } = VBObjectType.TypeInfo;
    public static VBType VbVariantType { get; } = VBVariantType.TypeInfo;
    public static VBType VbEmptyType { get; } = VBEmptyType.TypeInfo;
    public static VBType VbNullType { get; } = VBNullType.TypeInfo;
    public static VBType VbErrorType { get; } = VBErrorType.TypeInfo;

    public static ImmutableArray<VBType> IntrinsicTypes { get; } =
        [
            VbBooleanType,
            VbByteType,
            VbIntegerType,
            VbLongType,
            VbLongLongType,
            VbCurrencyType,
            VbDecimalType,
            VbSingleType,
            VbDoubleType,
            VbDateType,
            VbStringType,
            VbObjectType,
            VbVariantType,
            VbEmptyType,
            VbNullType,
            VbErrorType,
        ];
    #endregion

    public VBType(Type? managedType, string name, bool isUserDefined = false, bool isHidden = false)
    {
        ManagedType = managedType;
        Name = name;
        IsUserDefined = isUserDefined;
        IsHidden = isHidden;
    }

    public Type? ManagedType { get; init; }
    public int? Size { get; init; }

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
    /// Whether this type can be passed by value.
    /// </summary>
    public virtual bool CanPassByValue { get; } = true;

    /// <summary>
    /// Override in derived types to specify VBTypes that are safe to convert this type into.
    /// </summary>
    public virtual VBType[] ConvertsSafelyToTypes { get; } = [];
}
