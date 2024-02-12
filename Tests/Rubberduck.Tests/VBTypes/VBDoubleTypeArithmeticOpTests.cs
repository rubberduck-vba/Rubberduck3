﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using System;

namespace Rubberduck.Tests.VBTypes;

public abstract class VBDoubleTypeArithmeticOpTests : OperatorTests
{
    protected abstract double ExpectResult(double lhs, double rhs);
    protected abstract DateTime ExpectResult(DateTime lhs, int rhs);

    protected sealed override VBUnaryOperator CreateOperator(Uri uri, TypedSymbol symbol) => throw new NotSupportedException();

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