using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBEmptyValue : VBTypedValue,
    IVBTypedValue<VBEmptyValue, IntPtr>, 
    INumericCoercion, 
    IStringCoercion
{
    public VBEmptyValue(TypedSymbol? symbol = null)
        : base(VBEmptyType.TypeInfo, symbol) { }

    public static VBEmptyValue Empty { get; } = new VBEmptyValue();

    public IntPtr Value => IntPtr.Zero;
    public VBEmptyValue DefaultValue => Empty;
    public IntPtr NominalValue => IntPtr.Zero;

    public override int Size => sizeof(int);

    public VBDoubleValue AsCoercedNumeric(int depth = 0) => VBDoubleValue.Zero;
    public VBStringValue AsCoercedString(int depth = 0) => VBStringValue.ZeroLengthString;
}
