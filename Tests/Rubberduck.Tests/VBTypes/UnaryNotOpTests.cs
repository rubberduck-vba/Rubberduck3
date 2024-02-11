using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using System;

namespace Rubberduck.Tests.VBTypes;

[TestClass]
public class UnaryNotOpTests : UnaryOperatorTests
{
    protected override VBUnaryOperator CreateOperator(Uri uri, TypedSymbol symbol) => new VBNotOperator(symbol.Name, uri, symbol);

    [TestMethod]
    [TestCategory("Operators")]
    [DataRow(true, false)]
    [DataRow(false, true)]
    public void LogicalOperations(bool lhsValue, bool expectedValue)
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;

        var symbol = CreateVariable(context, VBBooleanType.TypeInfo, "LHS");
        context.SetSymbolValue(symbol, new VBBooleanValue(symbol) { Value = lhsValue });

        var sut = CreateOperator(ref procedure, symbol);
        var scope = context.EnterScope(procedure);

        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNotNull(result);
        Assert.IsTrue(result is VBBooleanValue, result.TypeInfo.Name);
        Assert.AreEqual(expectedValue, ((VBBooleanValue)result).Value);
    }

    [TestMethod]
    [TestCategory("Operators")]
    [DataRow(-1, 0)]
    [DataRow(0, -1)]
    [DataRow(42, -43)]
    [DataRow(1, -2)]
    public void BitwiseOperations(int lhsValue, int expectedValue)
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;

        var symbol = CreateVariable(context, VBIntegerType.TypeInfo, "LHS");
        context.SetSymbolValue(symbol, new VBIntegerValue(symbol) { NumericValue = lhsValue });

        var sut = CreateOperator(ref procedure, symbol);
        var scope = context.EnterScope(procedure);

        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNotNull(result);
        Assert.IsTrue(result is VBLongValue, result.TypeInfo.Name);
        Assert.AreEqual(expectedValue, ((VBLongValue)result).Value);
    }
}
