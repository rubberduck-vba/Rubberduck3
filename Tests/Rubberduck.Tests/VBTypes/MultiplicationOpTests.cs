using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Operators;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using System;

namespace Rubberduck.Tests.VBTypes;

[TestClass]
public class MultiplicationOpTests : WideningArithmeticOpTests
{
    protected override LargestVBType Mode => LargestVBType.FromOperands;

    protected override VBBinaryOperator CreateOperator(Uri uri, TypedSymbol lhs, TypedSymbol rhs) => new VBMultiplicationOperator(uri, lhs.Name, rhs.Name, lhs, rhs);
    protected override double ExpectResult(double lhs, double rhs) => lhs * rhs;

    [TestMethod]
    [TestCategory("Operators")]
    [DataRow(2, "2024-01-01")]
    [DataRow(-2, "2024-01-01")]
    [DataRow(0, "2024-01-01")]
    public override void GivenVBDateRHS_NumericLHS_ReturnsVBDateValue(int lhsValue, string rhsDateTime)
    {
        var rhsValue = DateTime.Parse(rhsDateTime);

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var lhs = CreateVariable(context, VBByteType.TypeInfo, "LHS");
        var rhs = CreateVariable(context, VBDateType.TypeInfo, "RHS");

        context.SetSymbolValue(lhs, new VBByteValue(lhs) { NumericValue = lhsValue });
        context.SetSymbolValue(rhs, new VBDateValue(rhs) { Value = rhsValue });

        var sut = CreateOperator(ref procedure, lhs, rhs);
        var scope = context.EnterScope(procedure);

        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNotNull(result);
        Assert.IsTrue(result is VBDoubleValue, result.TypeInfo.Name);
        Assert.AreEqual(ExpectResult(lhsValue, rhsValue.ToOADate()), ((VBDoubleValue)result).Value);
    }

    [TestMethod]
    [TestCategory("Operators")]
    [DataRow("2024-01-01", 2)]
    [DataRow("2024-01-01", -2)]
    [DataRow("2024-01-01", 0)]
    public override void GivenVBDateLHS_NumericRHS_ReturnsVBDateValue(string lhsDateTime, int rhsValue)
    {
        var lhsValue = DateTime.Parse(lhsDateTime);

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var lhs = CreateVariable(context, VBDateType.TypeInfo, "LHS");
        var rhs = CreateVariable(context, VBByteType.TypeInfo, "RHS");

        context.SetSymbolValue(lhs, new VBDateValue(lhs) { Value = lhsValue });
        context.SetSymbolValue(rhs, new VBByteValue(rhs) { NumericValue = rhsValue });

        var sut = CreateOperator(ref procedure, lhs, rhs);
        var scope = context.EnterScope(procedure);

        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNotNull(result);
        Assert.IsTrue(result is VBDoubleValue, result.TypeInfo.Name);
        Assert.AreEqual(ExpectResult<double>(lhsValue, rhsValue), ((INumericValue)result).AsDouble().Value);
    }

    protected override T ExpectResult<T>(DateTime lhs, int rhs) => (T)Convert.ChangeType(lhs.ToOADate() * rhs, typeof(T));
}