using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBObjectValue : VBTypedValue, IVBTypedValue<object?>, INumericCoercion, IStringCoercion
{
    public VBObjectValue(TypedSymbol? declarationSymbol = null)
        : base(VBObjectType.TypeInfo, declarationSymbol) { }

    public object? Value { get; } = default;
    public object? DefaultValue { get; } = default;

    public double? AsCoercedNumeric(int depth = 0) => LetCoerce(depth) is INumericValue value ? value.AsDouble() : null;
    public string? AsCoercedString(int depth = 0) => LetCoerce(depth) is VBStringValue value ? value.Value : null;

    /// <summary>
    /// Implicit default member call coerces the object reference into an intrinsic value.
    /// </summary>
    /// <remarks>
    /// Let coercion is recursive: a class type's default member may be another class type with a default member.
    /// </remarks>
    public VBTypedValue? LetCoerce(int depth = 0)
    {
        if (depth >= 9) // TODO configure
        {
            // TODO issue a diagnostic for this
            throw VBRuntimeErrorException.OutOfStackSpace;
        }

        if (TypeInfo is VBClassType classType && classType.DefaultMember != null)
        {
            var symbol = classType.DefaultMember.Declaration as TypedSymbol;
            if (classType.DefaultMember.ResolvedType is INumericCoercion coercibleNumeric)
            {
                var value = coercibleNumeric.AsCoercedNumeric(depth);
                if (symbol != null && value != null)
                {
                    return new VBDoubleValue(symbol).WithValue(value.Value);
                }
            }
            else if (classType.DefaultMember.ResolvedType is IStringCoercion coercibleString)
            {
                var value = coercibleString.AsCoercedString(depth);
                if (symbol != null && value != null)
                {
                    return new VBStringValue(symbol).WithValue(value);
                }
            }
        }
        return null;
    }
}
