﻿using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBCurrencyType : VBIntrinsicType<decimal>
{
    private VBCurrencyType() : base(Tokens.Currency) { }
    public static VBCurrencyType TypeInfo { get; } = new();

    public override decimal DefaultValue { get; }
    public override VBType[] ConvertsSafelyToTypes { get; }
        = [VbDecimalType, VbSingleType, VbDoubleType, VbStringType, VbVariantType];
}
