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

public abstract class ArithmeticOpTests : OperatorTests
{
    protected VBBinaryOperator CreateOperator(ref VBProcedureMember scope, TypedSymbol lhs, TypedSymbol rhs)
    {
        var procedureSymbol = ParentProcedureSymbol.WithChildren([lhs, rhs]);
        var parentProcedure = ParentProcedure.WithDeclaration(procedureSymbol);
        return CreateOperator(parentProcedure.Uri, lhs, rhs);
    }

    protected abstract VBBinaryOperator CreateOperator(Uri uri, TypedSymbol lhs, TypedSymbol rhs);
}

[TestClass]
public class AdditionOpTests : ArithmeticOpTests
{
    protected override VBBinaryOperator CreateOperator(Uri uri, TypedSymbol lhs, TypedSymbol rhs) => new VBAdditionOperator(uri, lhs.Name, rhs.Name, lhs, rhs);

    [TestMethod]
    [TestCategory("Operators")]
    [DataRow(2, 2)]
    [DataRow(-2, 2)]
    [DataRow(2, -2)]
    [DataRow(0, 2)]
    [DataRow(2, 0)]
    [DataRow(0, 0)]
    public void GivenVBByteVBInteger_ReturnsLargestVBTypeRHS(int lhsValue, int rhsValue)
    {
        var expected = lhsValue + rhsValue;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var lhs = CreateVariable(context, VBByteType.TypeInfo, "LHS");
        var rhs = CreateVariable(context, VBIntegerType.TypeInfo, "RHS");

        context.SetSymbolValue(lhs, new VBByteValue(lhs) { NumericValue = lhsValue });
        context.SetSymbolValue(rhs, new VBIntegerValue(rhs) { NumericValue = rhsValue });

        var sut = CreateOperator(ref procedure, lhs, rhs);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNotNull(result);
        Assert.IsTrue(result is VBIntegerValue, result.TypeInfo.Name);
        Assert.AreEqual(expected, ((VBIntegerValue)result).NumericValue);
    }

    [TestMethod]
    [TestCategory("Operators")]
    [DataRow(2, 2)]
    [DataRow(-2, 2)]
    [DataRow(2, -2)]
    [DataRow(0, 2)]
    [DataRow(2, 0)]
    [DataRow(0, 0)]
    public void GivenVBByteVBInteger_ReturnsLargestVBTypeLHS(int lhsValue, int rhsValue)
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var lhs = CreateVariable(context, VBLongType.TypeInfo, "LHS");
        var rhs = CreateVariable(context, VBByteType.TypeInfo, "RHS");

        context.SetSymbolValue(lhs, new VBLongValue(lhs) { NumericValue = lhsValue });
        context.SetSymbolValue(rhs, new VBByteValue(rhs) { NumericValue = rhsValue });

        var sut = CreateOperator(ref procedure, lhs, rhs);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNotNull(result);
        Assert.IsTrue(result is VBLongValue, result.TypeInfo.Name);
        Assert.AreEqual(lhsValue + rhsValue, ((VBLongValue)result).NumericValue);
    }

    [TestMethod]
    [TestCategory("Operators")]
    [DataRow("2024-01-01", 2)]
    [DataRow("2024-01-01", -2)]
    [DataRow("2024-01-01", 0)]
    public void GivenVBDateLHS_NumericRHS_ReturnsVBDateValue(string lhsDateTime, int rhsValue)
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
        Assert.IsTrue(result is VBDateValue, result.TypeInfo.Name);
        Assert.AreEqual(lhsValue.AddDays(rhsValue), ((VBDateValue)result).Value);
    }

    [TestMethod]
    [TestCategory("Operators")]
    [DataRow(2, "2024-01-01")]
    [DataRow(-2, "2024-01-01")]
    [DataRow(0, "2024-01-01")]
    public void GivenVBDateRHS_NumericLHS_ReturnsVBDateValue(int lhsValue, string rhsDateTime)
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
        Assert.IsTrue(result is VBDateValue, result.TypeInfo.Name);
        Assert.AreEqual(rhsValue.AddDays(lhsValue), ((VBDateValue)result).Value);
    }

    [TestMethod]
    [TestCategory("Operators")]
    [DataRow(200, 200)]
    [DataRow(255, 1)]
    [DataRow(999, 0)]
    [DataRow(0, 999)]
    public void GivenVBByteOperands_TotalOverflows_ThrowsOverflowError(int lhsValue, int rhsValue)
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var lhs = CreateVariable(context, VBByteType.TypeInfo, "LHS");
        var rhs = CreateVariable(context, VBByteType.TypeInfo, "RHS");

        context.SetSymbolValue(lhs, new VBByteValue(lhs) { NumericValue = lhsValue });
        context.SetSymbolValue(rhs, new VBByteValue(rhs) { NumericValue = rhsValue });

        var sut = CreateOperator(ref procedure, lhs, rhs);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(result, result?.TypeInfo.Name);
        Assert.IsTrue(scope.Error is VBRuntimeErrorException);
        Assert.AreEqual(VBRuntimeErrorException.Overflow(sut).VBErrorNumber, scope.Error.VBErrorNumber);
    }

    [TestMethod]
    [TestCategory("Operators")]
    [DataRow(32750, 32750)]
    [DataRow(32767, 1)]
    [DataRow(99999, 0)]
    [DataRow(0, 99999)]
    public void GivenVBIntegerOperands_TotalOverflows_ThrowsOverflowError(int lhsValue, int rhsValue)
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var lhs = CreateVariable(context, VBIntegerType.TypeInfo, "LHS");
        var rhs = CreateVariable(context, VBIntegerType.TypeInfo, "RHS");

        context.SetSymbolValue(lhs, new VBIntegerValue(lhs) { NumericValue = lhsValue });
        context.SetSymbolValue(rhs, new VBIntegerValue(rhs) { NumericValue = rhsValue });

        var sut = CreateOperator(ref procedure, lhs, rhs);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(result, result?.TypeInfo.Name);
        Assert.IsTrue(scope.Error is VBRuntimeErrorException);
        Assert.AreEqual(VBRuntimeErrorException.Overflow(sut).VBErrorNumber, scope.Error.VBErrorNumber);
    }

    [TestMethod]
    [TestCategory("Operators")]
    [DataRow("2", 2)]
    [DataRow("-2", 2)]
    [DataRow("2", -2)]
    [DataRow("0", 2)]
    [DataRow("2", 0)]
    [DataRow("0", 0)]
    [DataRow("&HFF", 0)]
    public void GivenCoercibleStringLHS_ReturnsVBDoubleValue(string lhsNumericString, int rhsValue)
    {
        if (!int.TryParse(lhsNumericString, out var lhsValue))
        {
            lhsValue = Convert.ToInt32(lhsNumericString.Replace("&H", "0x"), 16);
        }

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var lhs = CreateVariable(context, VBStringType.TypeInfo, "LHS");
        var rhs = CreateVariable(context, VBIntegerType.TypeInfo, "RHS");

        context.SetSymbolValue(lhs, new VBStringValue(lhs) { Value = lhsNumericString });
        context.SetSymbolValue(rhs, new VBIntegerValue(rhs) { NumericValue = rhsValue });

        var sut = CreateOperator(ref procedure, lhs, rhs);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNotNull(result, result?.TypeInfo.Name);
        Assert.IsTrue(result is VBDoubleValue, result.TypeInfo.Name);
        Assert.AreEqual(lhsValue + rhsValue, ((VBDoubleValue)result).NumericValue);
    }

    [TestMethod]
    [TestCategory("Operators")]
    [DataRow("2A", 2)]
    [DataRow("A2", 2)]
    [DataRow("2O", -2)]
    [DataRow("1%", 2)]
    [DataRow("ZZ", 0)]
    [DataRow("NaN", 0)]
    public void GivenNonNumericStringLHS_ThrowsError13(string lhsStringValue, int rhsValue)
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var lhs = CreateVariable(context, VBStringType.TypeInfo, "LHS");
        var rhs = CreateVariable(context, VBIntegerType.TypeInfo, "RHS");

        context.SetSymbolValue(lhs, new VBStringValue(lhs) { Value = lhsStringValue });
        context.SetSymbolValue(rhs, new VBIntegerValue(rhs) { NumericValue = rhsValue });

        var sut = CreateOperator(ref procedure, lhs, rhs);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(result, result?.TypeInfo.Name);
        Assert.IsTrue(scope.Error is VBRuntimeErrorException);
        Assert.AreEqual(13, scope.Error.VBErrorNumber);
    }

    [TestMethod]
    [TestCategory("Operators")]
    [DataRow(2)]
    [DataRow(-2)]
    [DataRow(0)]
    public void GivenVBNullValueLHS_ReturnsVBNullValue(int rhsValue)
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var lhs = CreateVariable(context, VBVariantType.TypeInfo, "LHS");
        var rhs = CreateVariable(context, VBIntegerType.TypeInfo, "RHS");

        context.SetSymbolValue(lhs, new VBVariantValue(VBNullValue.Null, lhs));
        context.SetSymbolValue(rhs, new VBIntegerValue(rhs) { NumericValue = rhsValue });

        var sut = CreateOperator(ref procedure, lhs, rhs);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNotNull(result, result?.TypeInfo.Name);
        Assert.IsTrue(result is VBNullValue, result.TypeInfo.Name);
    }

    [TestMethod]
    [TestCategory("Operators")]
    [DataRow(2)]
    [DataRow(-2)]
    [DataRow(0)]
    public void GivenVBNullValueRHS_ReturnsVBNullValue(int lhsValue)
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var lhs = CreateVariable(context, VBIntegerType.TypeInfo, "LHS");
        var rhs = CreateVariable(context, VBVariantType.TypeInfo, "RHS");

        context.SetSymbolValue(lhs, new VBIntegerValue(lhs) { NumericValue = lhsValue });
        context.SetSymbolValue(rhs, new VBVariantValue(VBNullValue.Null, rhs));

        var sut = CreateOperator(ref procedure, lhs, rhs);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNotNull(result, result?.TypeInfo.Name);
        Assert.IsTrue(result is VBNullValue, result.TypeInfo.Name);
    }

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
