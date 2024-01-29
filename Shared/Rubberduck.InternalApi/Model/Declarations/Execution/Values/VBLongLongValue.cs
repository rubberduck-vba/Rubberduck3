using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBLongLongValue : VBTypedValue, IVBTypedValue<long>, INumericValue, INumericCoercion, IStringCoercion
{
    public VBLongLongValue(TypedSymbol? declarationSymbol = null) 
        : base(VBLongLongType.TypeInfo, declarationSymbol) { }

    public long Value { get; init; } = default;
    public long DefaultValue { get; } = default;

    public double? AsCoercedNumeric(int depth = 0) => AsDouble();
    public string? AsCoercedString(int depth = 0) => Value.ToString();
    public double AsDouble() => (double)Value;
    public VBTypedValue WithValue(double value) => this with { Value = (long)value };
}
