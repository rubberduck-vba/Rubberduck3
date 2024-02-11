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
public class NegationOperatorTests : UnaryOperatorTests
{
    protected override VBUnaryOperator CreateOperator(Uri uri, TypedSymbol symbol) => new VBNegationOperator(expression: $"-{symbol.Name}", symbol, uri);

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenVBByteValue_ReturnsNegativeVBIntegerValue()
    {
        var initialValue = 42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBByteType.TypeInfo, "TEST");
        
        var value = new VBByteValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBIntegerValue);
        Assert.AreEqual(-initialValue, ((VBIntegerValue)result).Value);
        Console.WriteLine(((VBIntegerValue)result).Value);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenVBByteZero_ReturnsVBIntegerZeroValue()
    {
        var initialValue = 0;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBByteType.TypeInfo, "TEST");
        
        var value = new VBByteValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.AreEqual(VBIntegerValue.Zero, result);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenPositiveVBLongPtrValue_32bit_ReturnsNegativeVBLongValue()
    {
        var initialValue = 42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        context.Is64BitHost = false;

        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBLongPtrType.TypeInfo, "TEST");
       
        var value = new VBLongPtrValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBLongValue, result.TypeInfo.Name);
        Assert.AreEqual(-initialValue, ((VBLongValue)result).Value);
        Console.WriteLine(((VBLongValue)result).Value);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenPositiveVBLongPtrValue_64bit_ReturnsNegativeVBLongLongValue()
    {
        var initialValue = 42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        context.Is64BitHost = true;

        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBLongPtrType.TypeInfo, "TEST");
        
        var value = new VBLongPtrValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);
        
        var sut = CreateOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBLongLongValue, result.TypeInfo.Name);
        Assert.AreEqual(-initialValue, ((VBLongLongValue)result).Value);
        Console.WriteLine(((VBLongLongValue)result).Value);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenPositiveNumericValue_ReturnsSameNumericType_VBInteger()
    {
        var initialValue = 42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBIntegerType.TypeInfo, "TEST");
        
        var value = new VBIntegerValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBIntegerValue);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenPositiveNumericValue_ReturnsSameNumericType_VBLong()
    {
        var initialValue = 42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBLongType.TypeInfo, "TEST");
        
        var value = new VBLongValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBLongValue);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenPositiveNumericValue_ReturnsSameNumericType_VBLongLong()
    {
        var initialValue = 42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBLongLongType.TypeInfo, "TEST");
        
        var value = new VBLongLongValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBLongLongValue);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenPositiveNumericValue_ReturnsSameNumericType_VBCurrency()
    {
        var initialValue = 42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBCurrencyType.TypeInfo, "TEST");
        
        var value = new VBCurrencyValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBCurrencyValue);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenPositiveNumericValue_ReturnsSameNumericType_VBDecimal()
    {
        var initialValue = 42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBDecimalType.TypeInfo, "TEST");
        
        var value = new VBDecimalValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBDecimalValue);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenPositiveNumericValue_ReturnsSameNumericType_VBSingle()
    {
        var initialValue = 42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBSingleType.TypeInfo, "TEST");
        
        var value = new VBSingleValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBSingleValue);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenPositiveNumericValue_ReturnsSameNumericType_VBDouble()
    {
        var initialValue = 42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBDoubleType.TypeInfo, "TEST");
        
        var value = new VBDoubleValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBDoubleValue);
    }


    [TestMethod]
    [TestCategory("Operators")]
    public void GivenNegativeVBLongPtrValue_32bit_ReturnsPositiveVBLongValue()
    {
        var initialValue = -42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        context.Is64BitHost = false;

        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBLongPtrType.TypeInfo, "TEST");
        
        var value = new VBLongPtrValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBLongValue, result.TypeInfo.Name);
        Assert.AreEqual(-initialValue, ((VBLongValue)result).Value);
        Console.WriteLine(((VBLongValue)result).Value);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenNegativeVBLongPtrValue_64bit_ReturnsPositiveVBLongLongValue()
    {
        var initialValue = -42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        context.Is64BitHost = true;

        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBLongPtrType.TypeInfo, "TEST");
        
        var value = new VBLongPtrValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBLongLongValue, result.TypeInfo.Name);
        Assert.AreEqual(-initialValue, ((VBLongLongValue)result).Value);
        Console.WriteLine(((VBLongLongValue)result).Value);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenNegativeNumericValue_ReturnsSameNumericType_VBInteger()
    {
        var initialValue = -42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBIntegerType.TypeInfo, "TEST");
        
        var value = new VBIntegerValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBIntegerValue);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenNegativeNumericValue_ReturnsSameNumericType_VBLong()
    {
        var initialValue = -42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBLongType.TypeInfo, "TEST");
        
        var value = new VBLongValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBLongValue);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenNegativeNumericValue_ReturnsSameNumericType_VBLongLong()
    {
        var initialValue = -42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBLongLongType.TypeInfo, "TEST");
        
        var value = new VBLongLongValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBLongLongValue);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenNegativeNumericValue_ReturnsSameNumericType_VBCurrency()
    {
        var initialValue = -42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBCurrencyType.TypeInfo, "TEST");
        
        var value = new VBCurrencyValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBCurrencyValue);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenNegativeNumericValue_ReturnsSameNumericType_VBDecimal()
    {
        var initialValue = -42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBDecimalType.TypeInfo, "TEST");
        
        var value = new VBDecimalValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBDecimalValue);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenNegativeNumericValue_ReturnsSameNumericType_VBSingle()
    {
        var initialValue = -42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBSingleType.TypeInfo, "TEST");
        
        var value = new VBSingleValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBSingleValue);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenNegativeNumericValue_ReturnsSameNumericType_VBDouble()
    {
        var initialValue = -42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBDoubleType.TypeInfo, "TEST");
        
        var value = new VBDoubleValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBDoubleValue);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenNothingObjectValue_ThrowsError91()
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBObjectType.TypeInfo, "TEST");

        var sut = CreateOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(result);
        Assert.IsTrue(scope.ActiveErrorState);
        Assert.AreEqual(VBRuntimeErrorException.ObjectVariableNotSet(variable).VBErrorNumber, scope.Error?.VBErrorNumber);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenObjectValueWithoutDefaultMember_ThrowsError438()
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBObjectType.TypeInfo, "TEST");
        
        var value = new VBObjectValue(variable) { Value = Guid.NewGuid() };
        context.SetSymbolValue(variable, value);

        var sut = CreateOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(result);
        Assert.IsTrue(scope.ActiveErrorState);
        Assert.AreEqual(VBRuntimeErrorException.ObjectDoesntSupportPropertyOrMethod(variable).VBErrorNumber, scope.Error?.VBErrorNumber);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenVBEmptyValue_ReturnsVBLongZero()
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBVariantType.TypeInfo, "TEST");

        var sut = CreateOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.AreEqual(VBLongValue.Zero, result);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenVBDateValue_ReturnsVBDateValue()
    {
        var initialValue = DateTime.Now;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBDateType.TypeInfo, "TEST");
        
        var value = new VBDateValue(variable) { Value = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBDateValue, result?.TypeInfo.Name);
    }

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenVBNullValue_ReturnsVBNullValue()
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBVariantType.TypeInfo, "TEST");

        var value = new VBNullValue(variable);
        context.SetSymbolValue(variable, value);

        var sut = CreateOperator(ref procedure, value.Symbol!);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBNullValue, result?.TypeInfo.Name);
    }
}
