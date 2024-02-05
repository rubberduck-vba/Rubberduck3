using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBErrorValue : VBTypedValue, IVBTypedValue<int?>
{
    public VBErrorValue(TypedSymbol? symbol, int? value = 0) : base(VBType.VbErrorType, symbol)
    {
        Value = value;
        DefaultValue = 0;
    }

    public int? Value { get; init; }

    public int? DefaultValue { get; init; }

    public string AsString => $"Error {Value}";
}