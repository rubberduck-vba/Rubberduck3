using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.Tests.VBTypes;
using System;

namespace Rubberduck.Tests.VBTypes;

[TestClass]
public class AdditionOpTests : WideningArithmeticOpTests
{
    protected override VBBinaryOperator CreateOperator(Uri uri, TypedSymbol lhs, TypedSymbol rhs) => new VBAdditionOperator(uri, lhs.Name, rhs.Name, lhs, rhs);
    protected override double ExpectResult(double lhs, double rhs) => lhs + rhs;
    protected override T ExpectResult<T>(DateTime lhs, int rhs) => (T)Convert.ChangeType(lhs.AddDays(rhs), typeof(T));

    [TestMethod]
    [TestCategory("Operators")]
    [DataRow("2", "2")]
    [DataRow("-2", "Z")]
    [DataRow("2", "-2")]
    [DataRow("0", "2")]
    [DataRow("2", "0")]
    [DataRow("A", "B")]
    [DataRow("&HFF", "0")]
    public void GivenVBStringOperands_ReturnsConcatenatedVBStringValue(string lhsValue, string rhsValue)
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var lhs = CreateVariable(context, VBStringType.TypeInfo, "LHS");
        var rhs = CreateVariable(context, VBStringType.TypeInfo, "RHS");

        context.SetSymbolValue(lhs, new VBStringValue(lhs) { Value = lhsValue });
        context.SetSymbolValue(rhs, new VBStringValue(rhs) { Value = rhsValue });

        var sut = CreateOperator(ref procedure, lhs, rhs);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNotNull(result);
        Assert.IsTrue(result is VBStringValue, result.TypeInfo.Name);
        Assert.AreEqual(lhsValue + rhsValue, ((VBStringValue)result).Value);
    }
}
