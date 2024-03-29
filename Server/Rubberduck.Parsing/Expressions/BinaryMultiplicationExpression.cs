﻿using Rubberduck.Parsing.Abstract;

namespace Rubberduck.Parsing.Expressions;

public sealed class BinaryMultiplicationExpression : Expression
{
    private readonly IExpression _left;
    private readonly IExpression _right;

    public BinaryMultiplicationExpression(IExpression left, IExpression right)
    {
        _left = left;
        _right = right;
    }

    public override IValue Evaluate()
    {
        var left = _left.Evaluate();
        var right = _right.Evaluate();
        if (left == null || right == null)
        {
            return null;
        }
        var leftValue = left.AsDecimal;
        var rightValue = right.AsDecimal;
        return new DecimalValue(leftValue * rightValue);
    }
}
