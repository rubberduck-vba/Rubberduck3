using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rubberduck.InternalApi.Model;
using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using System;
using System.Windows.Xps.Serialization;

namespace Rubberduck.Tests.VBTypes;

[TestClass]
public class CompareAndOpTests : OperatorTests
{
    protected override VBUnaryOperator CreateOperator(Uri uri, TypedSymbol symbol) => throw new NotSupportedException();
    protected override VBBinaryOperator CreateOperator(Uri uri, TypedSymbol lhs, TypedSymbol rhs) =>
        new VBAndOperator(Tokens.And, uri, lhs.Name, rhs.Name, lhs, rhs);

    [TestMethod]
    [TestCategory("Operators")]
    [DataRow(true, true, true)]
    [DataRow(false, true, false)]
    [DataRow(true, false, false)]
    [DataRow(false, false, false)]
    public void LogicalOperations(bool lhsValue, bool rhsValue, bool expected)
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var lhs = CreateVariable(context, VBBooleanType.TypeInfo, "LHS");
        var rhs = CreateVariable(context, VBBooleanType.TypeInfo, "RHS");

        context.SetSymbolValue(lhs, new VBBooleanValue(lhs) { Value = lhsValue });
        context.SetSymbolValue(rhs, new VBBooleanValue(rhs) { Value = rhsValue });

        var sut = CreateOperator(ref procedure, lhs, rhs);
        var scope = context.EnterScope(procedure);

        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNotNull(result);
        Assert.IsTrue(result is VBBooleanValue, result.TypeInfo.Name);
        Assert.AreEqual(expected, ((VBBooleanValue)result).Value);
    }

    [TestMethod]
    [TestCategory("Operators")]
    [DataRow(-1, -1, -1)]
    [DataRow(0, -1, 0)]
    [DataRow(0, 0, 0)]
    [DataRow(-1, 0, 0)]
    [DataRow(3, 7, 3)]
    [DataRow(1, -2, 0)]
    [DataRow(3, 7, 3)]
    [DataRow(3, -9, 3)]
    [DataRow(1, 1, 1)]
    public void BitwiseOperations(int lhsValue, int rhsValue, int expectedValue)
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;

        var lhsSymbol = CreateVariable(context, VBIntegerType.TypeInfo, "LHS");
        context.SetSymbolValue(lhsSymbol, new VBIntegerValue(lhsSymbol) { NumericValue = lhsValue });

        var rhsSymbol = CreateVariable(context, VBIntegerType.TypeInfo, "RHS");
        context.SetSymbolValue(rhsSymbol, new VBIntegerValue(rhsSymbol) { NumericValue = rhsValue });

        var sut = CreateOperator(ref procedure, lhsSymbol, rhsSymbol);
        var scope = context.EnterScope(procedure);

        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNotNull(result);
        Assert.IsTrue(result is VBLongValue, result.TypeInfo.Name);
        Assert.AreEqual(expectedValue, ((VBLongValue)result).Value);
    }
}

[TestClass]
public class CompareOrOpTests : OperatorTests
{
    protected override VBUnaryOperator CreateOperator(Uri uri, TypedSymbol symbol) => throw new NotSupportedException();
    protected override VBBinaryOperator CreateOperator(Uri uri, TypedSymbol lhs, TypedSymbol rhs) =>
        new VBOrOperator(Tokens.And, uri, lhs.Name, rhs.Name, lhs, rhs);

    [TestMethod]
    [TestCategory("Operators")]
    [DataRow(true, true, true)]
    [DataRow(false, true, true)]
    [DataRow(true, false, true)]
    [DataRow(false, false, false)]
    public void LogicalOperations(bool lhsValue, bool rhsValue, bool expected)
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var lhs = CreateVariable(context, VBBooleanType.TypeInfo, "LHS");
        var rhs = CreateVariable(context, VBBooleanType.TypeInfo, "RHS");

        context.SetSymbolValue(lhs, new VBBooleanValue(lhs) { Value = lhsValue });
        context.SetSymbolValue(rhs, new VBBooleanValue(rhs) { Value = rhsValue });

        var sut = CreateOperator(ref procedure, lhs, rhs);
        var scope = context.EnterScope(procedure);

        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNotNull(result);
        Assert.IsTrue(result is VBBooleanValue, result.TypeInfo.Name);
        Assert.AreEqual(expected, ((VBBooleanValue)result).Value);
    }
}