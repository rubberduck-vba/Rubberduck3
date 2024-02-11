using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBCurrencyType : VBIntrinsicType<decimal>, INumericType
{
    private static readonly VBCurrencyType _type = new();

    private VBCurrencyType() : base(Tokens.Currency) { }

    public static VBCurrencyType TypeInfo => _type;

    public override VBTypedValue DefaultValue { get; } = VBCurrencyValue.Zero;
}
