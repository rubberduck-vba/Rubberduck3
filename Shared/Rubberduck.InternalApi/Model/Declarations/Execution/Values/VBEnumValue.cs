using Rubberduck.InternalApi.Model.Declarations.Symbols;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBEnumValue : VBTypedValue, IVBTypedValue<VBLongValue?>, INumericCoercion, IStringCoercion
{
    public VBEnumValue(VBLongValue value, EnumMemberSymbol declarationSymbol) 
        : base(value.TypeInfo, declarationSymbol) 
    {
    }

    public double? AsCoercedNumeric(int depth = 0) => Value?.AsCoercedNumeric();
    public string? AsCoercedString(int depth = 0) => Value?.AsCoercedString();
    public VBLongValue? Value { get; }
    public VBLongValue? DefaultValue { get; }
}