﻿using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using System;

namespace Rubberduck.Tests.VBTypes;

public abstract class UnaryOperatorTests : OperatorTests
{
    protected sealed override VBBinaryOperator CreateOperator(Uri uri, TypedSymbol lhs, TypedSymbol rhs) => throw new NotSupportedException();
}
