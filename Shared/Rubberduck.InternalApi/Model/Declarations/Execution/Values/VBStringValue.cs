﻿using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBStringValue : VBTypedValue, IVBTypedValue<string?>, INumericCoercion
{
    public static VBStringValue VBNullString { get; } = new VBStringValue();

    public VBStringValue(TypedSymbol? declarationSymbol = null) 
        : base(VBStringType.TypeInfo, declarationSymbol) { }

    public double? AsCoercedNumeric() => double.TryParse(Value, out var numericValue) ? numericValue : null;
    public string? AsCoercedString() => string.Empty;
    public string? Value { get; init; } = default;
    public string? DefaultValue { get; } = default;

    public VBTypedValue WithValue(string value) => this with { Value = value };

    public double? AsCoercedNumeric(int depth = 0)
    {
        if (double.TryParse(Value, out var coerced))
        {
            return coerced;
        }

        throw VBRuntimeErrorException.TypeMismatch(Symbol!, $"Numeric coercion failed to coerce \"{Value}\" to a numeric value.");
    }
}
