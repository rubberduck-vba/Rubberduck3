using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBUserDefinedTypeValue : VBTypedValue, IVBTypedValue<object>
{
    public VBUserDefinedTypeValue(VBUserDefinedType type, TypedSymbol? declarationSymbol = null) 
        : base(type, declarationSymbol)
    {
        DefaultValue = new object();
        Value = DefaultValue;
    }

    public object Value { get; }
    public object DefaultValue { get; }
}
