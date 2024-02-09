using Rubberduck.InternalApi.Model.Declarations.Symbols;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBFixedStringValue : VBStringValue
{
    public VBFixedStringValue(int length, TypedSymbol? symbol = null)
        : base(symbol)
    {
        Length = length;
    }

    public override int Length { get; }

    public override VBStringValue WithValue(string? value)
    {
        value ??= string.Empty;
        if (value.Length > Length)
        {
            // here VBA silently truncates the value, where specs say the string shouldn't be assignable.
            // we should issue a diagnostic about the truncated string, but we don't have an execution context here.
            // ...and throwing a VBRuntimeErrorException would be a break vs what the VBE does, so
            // it's on *callers* to issue a diagnostic if the resulting string differs from the input value.
            value = value[..Length];
        }
        else if (value.Length < Length)
        {
            // here VBA silently pads the value with Chr(32), where specs say the string shouldn't be assignable.
            // same as above, except issue a diagnostic about the implicit padding instead of implicit truncation.
            value = value.PadRight(Length, ' ');
        }

        return this with { Value = value };
    }
}