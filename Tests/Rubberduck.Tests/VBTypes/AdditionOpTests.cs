using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using System;

namespace Rubberduck.Tests.VBTypes;

[TestClass]
public class AdditionOpTests : OperatorTests
{
    public VBAdditionOperator CreateAdditionOperator(ref VBProcedureMember scope, TypedSymbol lhs, TypedSymbol rhs)
    {
        var procedureSymbol = ParentProcedureSymbol.WithChildren([lhs, rhs]);
        var parentProcedure = ParentProcedure.WithDeclaration(procedureSymbol);

        return new VBAdditionOperator(parentProcedure.Uri, lhs.Name, rhs.Name, lhs, rhs);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenVBByteVBInteger_ReturnsLargestVBTypeRHS()
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var lhs = CreateVariable(context, VBByteType.TypeInfo, "LHS");
        var rhs = CreateVariable(context, VBIntegerType.TypeInfo, "RHS");

        context.SetSymbolValue(lhs, new VBByteValue(lhs) { NumericValue = 42 });
        context.SetSymbolValue(rhs, new VBIntegerValue(rhs) { NumericValue = 12 });

        var sut = CreateAdditionOperator(ref procedure, lhs, rhs);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNotNull(result);
        Assert.IsTrue(result is VBIntegerValue, result.TypeInfo.Name);
        Assert.AreEqual(42 + 12, ((VBIntegerValue)result).NumericValue);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenVBByteVBInteger_ReturnsLargestVBTypeLHS()
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var lhs = CreateVariable(context, VBLongType.TypeInfo, "LHS");
        var rhs = CreateVariable(context, VBByteType.TypeInfo, "RHS");

        context.SetSymbolValue(lhs, new VBLongValue(lhs) { NumericValue = 42 });
        context.SetSymbolValue(rhs, new VBByteValue(rhs) { NumericValue = 12 });

        var sut = CreateAdditionOperator(ref procedure, lhs, rhs);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNotNull(result);
        Assert.IsTrue(result is VBLongValue, result.TypeInfo.Name);
        Assert.AreEqual(42 + 12, ((VBLongValue)result).NumericValue);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenVBDateLHS_ReturnsVBDateValue()
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var lhs = CreateVariable(context, VBDateType.TypeInfo, "LHS");
        var rhs = CreateVariable(context, VBByteType.TypeInfo, "RHS");

        context.SetSymbolValue(lhs, new VBDateValue(lhs) { Value = DateTime.Today });
        context.SetSymbolValue(rhs, new VBByteValue(rhs) { NumericValue = 42 });

        var sut = CreateAdditionOperator(ref procedure, lhs, rhs);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNotNull(result);
        Assert.IsTrue(result is VBDateValue, result.TypeInfo.Name);
        Assert.AreEqual(DateTime.Today.AddDays(42), ((VBDateValue)result).Value);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenVBDateRHS_ReturnsVBDateValue()
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var lhs = CreateVariable(context, VBByteType.TypeInfo, "LHS");
        var rhs = CreateVariable(context, VBDateType.TypeInfo, "RHS");

        context.SetSymbolValue(lhs, new VBByteValue(lhs) { NumericValue = 42 });
        context.SetSymbolValue(rhs, new VBDateValue(rhs) { Value = DateTime.Today });

        var sut = CreateAdditionOperator(ref procedure, lhs, rhs);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNotNull(result);
        Assert.IsTrue(result is VBDateValue, result.TypeInfo.Name);
        Assert.AreEqual(DateTime.Today.AddDays(42), ((VBDateValue)result).Value);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenVBByteOperands_TotalOverflows_ThrowsOverflowError()
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var lhs = CreateVariable(context, VBByteType.TypeInfo, "LHS");
        var rhs = CreateVariable(context, VBByteType.TypeInfo, "RHS");

        context.SetSymbolValue(lhs, new VBByteValue(lhs) { NumericValue = VBByteValue.MaxValue.Value });
        context.SetSymbolValue(rhs, new VBByteValue(rhs) { NumericValue = VBByteValue.MaxValue.Value });

        var sut = CreateAdditionOperator(ref procedure, lhs, rhs);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(result, result?.TypeInfo.Name);
        Assert.IsTrue(scope.Error is VBRuntimeErrorException);
        Assert.AreEqual(6, scope.Error.VBErrorNumber);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenVBIntegerOperands_TotalOverflows_ThrowsOverflowError()
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var lhs = CreateVariable(context, VBIntegerType.TypeInfo, "LHS");
        var rhs = CreateVariable(context, VBIntegerType.TypeInfo, "RHS");

        context.SetSymbolValue(lhs, new VBIntegerValue(lhs) { NumericValue = VBIntegerValue.MaxValue.Value });
        context.SetSymbolValue(rhs, new VBIntegerValue(rhs) { NumericValue = VBIntegerValue.MaxValue.Value });

        var sut = CreateAdditionOperator(ref procedure, lhs, rhs);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(result, result?.TypeInfo.Name);
        Assert.IsTrue(scope.Error is VBRuntimeErrorException);
        Assert.AreEqual(6, scope.Error.VBErrorNumber);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenCoercibleStringLHS_ReturnsVBDoubleValue()
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var lhs = CreateVariable(context, VBStringType.TypeInfo, "LHS");
        var rhs = CreateVariable(context, VBIntegerType.TypeInfo, "RHS");

        context.SetSymbolValue(lhs, new VBStringValue(lhs) { Value = "42" });
        context.SetSymbolValue(rhs, new VBIntegerValue(rhs) { NumericValue = 10 });

        var sut = CreateAdditionOperator(ref procedure, lhs, rhs);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNotNull(result, result?.TypeInfo.Name);
        Assert.IsTrue(result is VBDoubleValue, result.TypeInfo.Name);
        Assert.AreEqual(42 + 10, ((VBDoubleValue)result).NumericValue);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenNonNumericStringLHS_ThrowsError13()
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var lhs = CreateVariable(context, VBStringType.TypeInfo, "LHS");
        var rhs = CreateVariable(context, VBIntegerType.TypeInfo, "RHS");

        context.SetSymbolValue(lhs, new VBStringValue(lhs) { Value = "NaN" }); // note: "NaN" parses as a legit .net double value
        context.SetSymbolValue(rhs, new VBIntegerValue(rhs) { NumericValue = 10 });

        var sut = CreateAdditionOperator(ref procedure, lhs, rhs);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(result, result?.TypeInfo.Name);
        Assert.IsTrue(scope.Error is VBRuntimeErrorException);
        Assert.AreEqual(13, scope.Error.VBErrorNumber);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenVBNullValueLHS_ReturnsVBNullValue()
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var lhs = CreateVariable(context, VBVariantType.TypeInfo, "LHS");
        var rhs = CreateVariable(context, VBIntegerType.TypeInfo, "RHS");

        context.SetSymbolValue(lhs, new VBVariantValue(VBNullValue.Null, lhs));
        context.SetSymbolValue(rhs, new VBIntegerValue(rhs) { NumericValue = 42 });

        var sut = CreateAdditionOperator(ref procedure, lhs, rhs);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNotNull(result, result?.TypeInfo.Name);
        Assert.IsTrue(result is VBNullValue, result.TypeInfo.Name);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenVBNullValueRHS_ReturnsVBNullValue()
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var lhs = CreateVariable(context, VBIntegerType.TypeInfo, "LHS");
        var rhs = CreateVariable(context, VBVariantType.TypeInfo, "RHS");

        context.SetSymbolValue(lhs, new VBIntegerValue(lhs) { NumericValue = 42 });
        context.SetSymbolValue(rhs, new VBVariantValue(VBNullValue.Null, rhs));

        var sut = CreateAdditionOperator(ref procedure, lhs, rhs);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNotNull(result, result?.TypeInfo.Name);
        Assert.IsTrue(result is VBNullValue, result.TypeInfo.Name);
    }
}
