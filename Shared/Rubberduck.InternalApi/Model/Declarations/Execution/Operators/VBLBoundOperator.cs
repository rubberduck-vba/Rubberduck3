using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBLBoundOperator : VBOperator
{
    public VBLBoundOperator(Uri parentUri, string arrayOperandExpression, string dimensionIndexExpression, TypedSymbol? resolvedArrayOperand = null, TypedSymbol? resolvedDimensionIndex = null)
        : base(Tokens.LBound, parentUri, (resolvedArrayOperand is null || resolvedDimensionIndex is null) ? null : [resolvedArrayOperand, resolvedDimensionIndex!])
    {
        ArrayOperandExpression = arrayOperandExpression;
        DimensionIndexExpression = dimensionIndexExpression;

        ResolvedArrayOperand = resolvedArrayOperand;
        ResolvedDimensionIndex = resolvedDimensionIndex;
    }

    public string ArrayOperandExpression { get; init; }
    public TypedSymbol? ResolvedArrayOperand { get; init; }

    public string DimensionIndexExpression { get; init; }
    public TypedSymbol? ResolvedDimensionIndex { get; init; }

    protected override VBTypedValue? EvaluateResult(ref VBExecutionScope context)
    {
        var array = context.GetTypedValue(ResolvedArrayOperand!) as VBArrayValue;
        var dimension = context.GetTypedValue(ResolvedDimensionIndex!) as VBNumericTypedValue;

        if (array != null)
        {
            var index = dimension?.AsLong().Value ?? 0;
            return new VBLongValue(ResolvedDimensionIndex).WithValue(array.Dimensions[index].LowerBound);
        }

        throw VBCompileErrorException.ExpectedArray(ResolvedArrayOperand!, "Use the `LBound` operator to find the lower boundary of an array variable.");
    }
}
