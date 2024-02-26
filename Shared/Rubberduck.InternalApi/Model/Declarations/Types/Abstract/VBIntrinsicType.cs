using System;
using System.Collections.Immutable;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

public abstract record class VBIntrinsicType : VBType
{
    public static ImmutableArray<VBType> IntrinsicTypes =>
        [
            VBBooleanType.TypeInfo,
            VBByteType.TypeInfo,
            VBIntegerType.TypeInfo,
            VBLongType.TypeInfo,
            VBLongLongType.TypeInfo,
            VBCurrencyType.TypeInfo,
            VBDecimalType.TypeInfo,
            VBSingleType.TypeInfo,
            VBDoubleType.TypeInfo,
            VBDateType.TypeInfo,
            VBStringType.TypeInfo,
            VBVariantType.TypeInfo,
            VBEmptyType.TypeInfo,
            VBNullType.TypeInfo,
            VBErrorType.TypeInfo,
        ];

    protected VBIntrinsicType(string name, Type managedType)
        : base(managedType, name, isUserDefined: false) { }

    /// <summary>
    /// <c>true</c> for all intrinsic types that are valid in an <c>AsTypeClause</c> declaration.
    /// </summary>
    public virtual bool IsDeclarable { get; } = true;

    /// <summary>
    /// Specifies a <em>type hint</em> character that can help typing literal values.
    /// </summary>
    public virtual char? TypeHintCharacter { get; }

    /// <summary>
    /// Specifies a <c>DefType</c> token that could really put this type system to the test.
    /// </summary>
    /// <remarks>
    /// Probably best to not implement this?
    /// </remarks>
    public virtual string? DefToken { get; }

    public override VBType[] ConvertsSafelyToTypes => IntrinsicTypes
    .Except([VBObjectType.TypeInfo, VBDateType.TypeInfo, VBErrorType.TypeInfo, VBNullType.TypeInfo, VBEmptyType.TypeInfo])
    .Where(t => t.DefaultValue.Size >= DefaultValue.Size)
    .ToArray();
}

public abstract record class VBIntrinsicType<T> : VBIntrinsicType
{
    protected VBIntrinsicType(string name) 
        : base(name, typeof(T))
    {
    }

    /// <summary>
    /// Attemps to resolve the specified <c>typeName</c> expression to an intrinsic type.
    /// </summary>
    /// <returns>
    /// <c>true</c> if resolution was successful and <c>type</c> is not <c>null</c>.
    /// </returns>
    public static bool TryResolve(string? typeName, out VBType? type)
    {
        type = IntrinsicTypes.SingleOrDefault(t => t.Name.Equals(typeName ?? Tokens.Variant, StringComparison.InvariantCultureIgnoreCase));
        return type != null;
    }
}
