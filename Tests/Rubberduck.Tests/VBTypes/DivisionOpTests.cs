using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Operators;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using System;

namespace Rubberduck.Tests.VBTypes;

[TestClass]
public class DivisionOpTests : VBDoubleTypeArithmeticOpTests
{
    protected override VBBinaryOperator CreateOperator(WorkspaceUri uri, TypedSymbol lhs, TypedSymbol rhs) => new VBDivisionOperator(uri, lhs.Name, rhs.Name, lhs, rhs);
    protected override double ExpectResult(double lhs, double rhs) => lhs / rhs;
    protected override DateTime ExpectResult(DateTime lhs, int rhs) => throw new NotSupportedException("LHS::VBDate returns a VBDouble");
}
