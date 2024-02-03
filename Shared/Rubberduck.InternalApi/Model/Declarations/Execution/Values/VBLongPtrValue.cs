using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBLongPtrValue : VBTypedValue, IVBTypedValue<int>, INumericCoercion, IStringCoercion
{
    public VBLongPtrValue(TypedSymbol? declarationSymbol = null) 
        : base(VBLongPtrType.TypeInfo, declarationSymbol) { }

    public double? AsCoercedNumeric(int depth = 0) => Value;
    public string? AsCoercedString(int depth = 0) => Value.ToString();
    public int Value { get; } = default;
    public int DefaultValue { get; } = default;
}
