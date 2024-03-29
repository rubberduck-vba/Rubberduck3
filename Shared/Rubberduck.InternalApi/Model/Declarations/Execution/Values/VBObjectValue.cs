﻿using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public record class VBObjectValue : VBTypedValue, 
    IVBTypedValue<VBObjectValue, Guid>, 
    INumericCoercion, 
    IStringCoercion
{
    public VBObjectValue(TypedSymbol? symbol)
        : base(VBObjectType.TypeInfo, symbol) { }

    public static VBObjectValue Nothing { get; } = new VBNothingValue();

    public Guid Value { get; init; }
    public override int Size => sizeof(int);

    public bool IsNothing() => Value == Nothing.Value;

    public VBDoubleValue? AsCoercedNumeric(int depth = 0) => LetCoerce(depth) is INumericValue value ? value.AsDouble() : null;
    public VBStringValue? AsCoercedString(int depth = 0) => LetCoerce(depth) is VBStringValue value ? value : null;

    /// <summary>
    /// Implicit default member call coerces the object reference into an intrinsic value.
    /// </summary>
    /// <remarks>
    /// Let coercion is recursive: a class type's default member may be another class type with a default member.
    /// </remarks>
    public VBTypedValue LetCoerce(int depth = 0)
    {
        if (depth >= 9) // TODO configure
        {
            throw VBRuntimeErrorException.OutOfStackSpace(Symbol!, $"Recursive `Let` coercion did not resolve a typed value, {depth} levels deep.");
        }

        if (IsNothing())
        {
            throw VBRuntimeErrorException.ObjectVariableNotSet(Symbol!, $"Recursive `Let` coercion requires the object reference to be assigned so that the default member can be invoked.");
        }

        if (TypeInfo is VBClassType classType && classType.DefaultMember != null)
        {
            var symbol = classType.DefaultMember.Declaration as TypedSymbol;
            if (classType.DefaultMember is VBReturningMember member)
            {
                if (member.ResolvedType is INumericCoercion coercibleNumeric)
                {
                    var value = coercibleNumeric.AsCoercedNumeric(depth);
                    if (symbol != null && value != null)
                    {
                        return new VBDoubleValue(symbol).WithValue(value.Value);
                    }
                }
                else if (member.ResolvedType is IStringCoercion coercibleString)
                {
                    var value = coercibleString.AsCoercedString(depth);
                    if (symbol != null && value != null)
                    {
                        return value;
                    }
                }
            }
        }
        throw VBRuntimeErrorException.ObjectDoesntSupportPropertyOrMethod(Symbol!, $"`Let` coercion requires an object type that defines a default member, but none was found.");
    }
}
