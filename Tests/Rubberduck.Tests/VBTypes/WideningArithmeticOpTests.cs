using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using System;

namespace Rubberduck.Tests.VBTypes;

public abstract class WideningArithmeticOpTests : OperatorTests
{
    protected enum LargestVBType
    {
        FromOperands,
        FromIntrinsic
    }

    protected sealed override VBUnaryOperator CreateOperator(WorkspaceUri uri, TypedSymbol symbol) => throw new NotSupportedException();

    protected abstract double ExpectResult(double lhs, double rhs);
    protected abstract T ExpectResult<T>(DateTime lhs, int rhs);

    protected virtual LargestVBType Mode => LargestVBType.FromOperands;

    [TestMethod]
    [TestCategory("Operators")]
    [DataRow(2, 2)]
    [DataRow(-2, 2)]
    [DataRow(2, -2)]
    [DataRow(0, 2)]
    [DataRow(2, 0)]
    [DataRow(0, 0)]
    public virtual void GivenVBByteVBInteger_ReturnsLargestVBTypeRHS(int lhsValue, int rhsValue)
    {
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
        Assert.IsTrue(Mode == LargestVBType.FromOperands ? result is VBIntegerValue : result is VBDoubleValue, result.TypeInfo.Name);
        Assert.AreEqual(ExpectResult(lhsValue, rhsValue), ((VBIntegerValue)result).NumericValue);
    }

    [TestMethod]
    [TestCategory("Operators")]
    [DataRow(2, 2)]
    [DataRow(-2, 2)]
    [DataRow(2, -2)]
    [DataRow(0, 2)]
    [DataRow(2, 0)]
    [DataRow(0, 0)]
    public virtual void GivenVBByteVBInteger_ReturnsLargestVBTypeLHS(int lhsValue, int rhsValue)
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var lhs = CreateVariable(context, VBLongType.TypeInfo, "LHS");
        var rhs = CreateVariable(context, VBByteType.TypeInfo, "RHS");

        context.SetSymbolValue(lhs, new VBLongValue(lhs) { NumericValue = lhsValue });
        context.SetSymbolValue(rhs, new VBByteValue(rhs) { NumericValue = rhsValue });

        var expectedType = Mode == LargestVBType.FromOperands ? VBLongType.TypeInfo : VBDoubleType.TypeInfo as VBType;
        var sut = CreateOperator(ref procedure, lhs, rhs);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNotNull(result);
        Assert.AreEqual(expectedType, result.TypeInfo);
        Assert.AreEqual(ExpectResult(lhsValue, rhsValue), ((INumericValue)result).AsDouble().Value);
    }

    [TestMethod]
    [TestCategory("Operators")]
    [DataRow("2024-01-01", 2)]
    [DataRow("2024-01-01", -2)]
    [DataRow("2024-01-01", 0)]
    public virtual void GivenVBDateLHS_NumericRHS_ReturnsVBDateValue(string lhsDateTime, int rhsValue)
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
        Assert.AreEqual(ExpectResult<DateTime>(lhsValue, rhsValue), ((VBDateValue)result).Value);
    }

    [TestMethod]
    [TestCategory("Operators")]
    [DataRow(2, "2024-01-01")]
    [DataRow(-2, "2024-01-01")]
    [DataRow(0, "2024-01-01")]
    public virtual void GivenVBDateRHS_NumericLHS_ReturnsVBDateValue(int lhsValue, string rhsDateTime)
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
        Assert.AreEqual(ExpectResult<DateTime>(rhsValue, lhsValue), ((VBDateValue)result).Value);
    }

    [TestMethod]
    [TestCategory("Operators")]
    [DataRow(32750, 32750)]
    [DataRow(32767, 2)]
    [DataRow(99999, 1)]
    [DataRow(99999, 99999)]
    public virtual void GivenVBIntegerOperands_OpOverflows_ThrowsOverflowError(int lhsValue, int rhsValue)
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

        if (Mode == LargestVBType.FromOperands)
        {
            Assert.IsNull(result, result?.TypeInfo.Name);
            Assert.IsTrue(scope.Error is VBRuntimeErrorException);
            Assert.AreEqual(VBRuntimeErrorException.Overflow(sut).VBErrorNumber, scope.Error.VBErrorNumber);
        }
        else
        {
            Assert.IsNotNull(result);
            Assert.IsNull(scope.Error);
            Assert.IsTrue(result is VBDoubleValue, result.TypeInfo.Name);
        }
    }

    [TestMethod]
    [TestCategory("Operators")]
    [DataRow("2", 2)]
    [DataRow("-2", 2)]
    [DataRow("2", -2)]
    [DataRow("0", 2)]
    [DataRow("2", 1)]
    [DataRow("0", 1)]
    [DataRow("&HFF", 1)]
    public virtual void GivenCoercibleStringLHS_ReturnsVBDoubleValue(string lhsNumericString, int rhsValue)
    {
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
    }

    [TestMethod]
    [TestCategory("Operators")]
    [DataRow("2A", 2)]
    [DataRow("A2", 2)]
    [DataRow("2O", -2)]
    [DataRow("1%", 2)]
    [DataRow("ZZ", 0)]
    [DataRow("NaN", 0)]
    public virtual void GivenNonNumericStringLHS_ThrowsError13(string lhsStringValue, int rhsValue)
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
        Assert.AreEqual(VBRuntimeErrorException.TypeMismatch(sut).VBErrorNumber, scope.Error.VBErrorNumber);
    }

    [TestMethod]
    [TestCategory("Operators")]
    [DataRow(2)]
    [DataRow(-2)]
    [DataRow(0)]
    public virtual void GivenVBNullValueLHS_ReturnsVBNullValue(int rhsValue)
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
    public virtual void GivenVBNullValueRHS_ReturnsVBNullValue(int lhsValue)
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
}
