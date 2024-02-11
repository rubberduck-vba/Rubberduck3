using Rubberduck.InternalApi.Model.Declarations.Symbols;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

/// <summary>
/// A special value that represents a VBType that is used in a value expression.
/// </summary>
/// <remarks>
/// Not used much beyond <c>TypeOf...Is</c> expressions.
/// </remarks>
public record class VBTypeDescValue : VBTypedValue
{
    public VBTypeDescValue(TypedSymbol symbol) 
        : base(symbol.ResolvedType!, symbol) { }

    public override int Size => sizeof(int);
}