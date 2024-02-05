using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Rubberduck.InternalApi.Model.Declarations.Operators;

public record class VBCompareLikeOperator : VBComparisonOperator
{
    public VBCompareLikeOperator(Uri parentUri, string lhsExpression, string rhsExpression, TypedSymbol? lhs = null, TypedSymbol? rhs = null)
        : base(Tokens.CompareLikeOp, parentUri, lhsExpression, rhsExpression, lhs, rhs)
    {
    }


    private static char[] LikePatternChars = ['?', '#', '*', '['];
    protected override VBTypedValue ExecuteBinaryOperator(ref VBExecutionScope context, VBTypedValue lhsValue, VBTypedValue rhsValue)
    {
        var lhsType = lhsValue.TypeInfo;
        var rhsType = rhsValue.TypeInfo;
        if (lhsType is VBNullType || rhsType is VBNullType)
        {
            return new VBNullValue(this);
        }

        string? lhsString;
        if (lhsType is VBStringType)
        {
            lhsString = ((VBStringValue)lhsValue).Value;
        }
        else if (lhsType is IStringCoercion coercible)
        {
            lhsString = coercible.AsCoercedString();
            context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitStringCoercion(lhsValue.Symbol!));
        }
        else
        {
            throw VBRuntimeErrorException.TypeMismatch(this, "LHS value expression did not resolve or coerce to a `String`.");
        }


        string? rhsPatternString;
        if (rhsType is VBStringType)
        {
            rhsPatternString = ((VBStringValue)rhsValue).Value!;
        }
        else if (rhsType is IStringCoercion coercible)
        {
            rhsPatternString = coercible.AsCoercedString()!;
            context = context.WithDiagnostic(RubberduckDiagnostic.ImplicitStringCoercion(lhsValue.Symbol!));
        }
        else
        {
            throw VBRuntimeErrorException.TypeMismatch(this, "RHS pattern expression did not resolve or coerce to a `String`.");
        }

        var builder = new StringBuilder();
        var isTokenGroup = false;

        // TODO test this extensively
        for (var i = 0; i < rhsPatternString.Length; i++)
        {
            var token = rhsPatternString[i];

            if (!LikePatternChars.Contains(token))
            {
                builder.Append(token);
            }
            else if (token == '?')
            {
                builder.Append('.');
            }
            else if (token == '#')
            {
                builder.Append(@"\d");
            }
            else if (token == '*')
            {
                builder.Append(@".*?");
            }
            else if (!isTokenGroup && token == '[' && i < rhsPatternString.Length - 1 && rhsPatternString[i + 1] != ']')
            {
                isTokenGroup = true;
                builder.Append('[');
            }
            else if (isTokenGroup && token == '!')
            {
                if (rhsPatternString[i - 1] == '[')
                {
                    builder.Append('^');
                }
            }
            else if (isTokenGroup && token == ']' && rhsPatternString[i - 1] != '[')
            {
                isTokenGroup = false;
                builder.Append(']');
            }
            else
            {
                builder.Append(token);
            }
        }

        if (isTokenGroup)
        {
            throw VBRuntimeErrorException.InvalidPatternString(rhsValue.Symbol!, "Character list '[...]' appears to be missing a ']' delimiter.");
        }

        var patternString = $"^{builder}$";
        return new VBBooleanValue(this) { Value = Regex.IsMatch(lhsString ?? string.Empty, patternString) };
    }
}
