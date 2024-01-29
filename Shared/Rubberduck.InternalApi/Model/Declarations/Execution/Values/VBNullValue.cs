using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBNullValue : VBTypedValue, IVBTypedValue<object?>, INumericCoercion, IStringCoercion
{
    public static VBNullValue Null { get; } = new VBNullValue();

    public VBNullValue(TypedSymbol? symbol = null) 
        : base(VBNullType.TypeInfo, symbol)
    {
    }

    public object? Value => null;

    public object? DefaultValue => null;

    public double? AsCoercedNumeric(int depth = 0) => 0;
    public string? AsCoercedString(int depth = 0) => string.Empty;
}
