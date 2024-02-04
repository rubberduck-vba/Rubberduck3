using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBEmptyValue : VBTypedValue, IVBTypedValue<int?>, INumericCoercion, IStringCoercion
{
    public static VBEmptyValue Empty { get; } = new VBEmptyValue(null as TypedSymbol);

    public VBEmptyValue(TypedSymbol? symbol)
        : base(VBEmptyType.TypeInfo, symbol)
    {
    }

    public int? Value => null;

    public int? DefaultValue => null;
    public double? AsCoercedNumeric(int depth = 0) => 0;
    public string? AsCoercedString(int depth = 0) => string.Empty;
}
