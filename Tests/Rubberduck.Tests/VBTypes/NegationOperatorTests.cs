using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rubberduck.InternalApi.Model;
using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System;
using System.IO.Abstractions;

using Visibility = Rubberduck.InternalApi.Model.Accessibility;

namespace Rubberduck.Tests.VBTypes;

[TestClass]
public class NegationOperatorTests : ServiceBaseTest
{
    private static readonly Uri ParentModuleUri = new Uri("test://project/module");
    private static readonly Uri ParentProcedureUri = new Uri("test://project/module/procedure");

    private static readonly ProcedureSymbol ParentProcedureSymbol = new("Procedure", ParentProcedureUri, Visibility.Public);
    private static VBProcedureMember ParentProcedure = new(ParentProcedureUri, "Procedure", RubberduckSymbolKind.Procedure, Visibility.Public, ParentProcedureSymbol, isUserDefined: true);
    private static readonly VBStdModuleType ParentModule = new("Module", ParentModuleUri, members: [ParentProcedure]);

    protected override void ConfigureServices(IServiceCollection services)
    {
        ((Mock<IFileSystem>)Mocks[typeof(IFileSystem)]).Setup(m => m.Path.Combine(It.IsAny<string>(), It.IsAny<string>())).Returns(() => "some path");

        base.ConfigureServices(services);
        services.AddScoped<VBExecutionContext>();
    }

    private TypedSymbol CreateVariable(VBExecutionContext context, VBType type, string name, 
        Visibility accessibility = Visibility.Implicit,
        string? asTypeToken = null)
    {
        var asType = asTypeToken is null ? null : $"{Tokens.As} {asTypeToken}";
        var symbol = new VariableDeclarationSymbol(name, ParentProcedureUri, accessibility, asType).WithResolvedType(type);

        context.SetSymbolValue(symbol, type.DefaultValue);
        return symbol;
    }

    private VBNegationOperator CreateNegationOperator(ref VBProcedureMember scope, TypedSymbol variable)
    {
        var procedureSymbol = ParentProcedureSymbol.WithChildren([variable]);
        var parentProcedure = ParentProcedure.WithDeclaration(procedureSymbol);

        return new VBNegationOperator(expression: $"-{variable.Name}", variable, parentProcedure.Uri);
    }

    [TestMethod]
    public void EvaluateResultGivenVBByteValue_ReturnsNegativeVBIntegerValue()
    {
        var initialValue = 42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBByteType.TypeInfo, "TEST");
        var value = new VBByteValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateNegationOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBIntegerValue);
        Assert.AreEqual(-initialValue, ((VBIntegerValue)result).Value);
        Console.WriteLine(((VBIntegerValue)result).Value);
    }

    [TestMethod]
    public void EvaluateResultGivenVBByteZero_ReturnsVBIntegerZeroValue()
    {
        var initialValue = 0;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBByteType.TypeInfo, "TEST");
        var value = new VBByteValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateNegationOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.AreEqual(VBIntegerValue.Zero, result);
    }

    [TestMethod]
    public void EvaluateResultGivenPositiveVBLongPtrValue_32bit_ReturnsNegativeVBLongValue()
    {
        var initialValue = 42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        context.Is64BitHost = false;

        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBLongPtrType.TypeInfo, "TEST");
        var value = new VBLongPtrValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateNegationOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBLongValue, result.TypeInfo.Name);
        Assert.AreEqual(-initialValue, ((VBLongValue)result).Value);
        Console.WriteLine(((VBLongValue)result).Value);
    }

    [TestMethod]
    public void EvaluateResultGivenPositiveVBLongPtrValue_64bit_ReturnsNegativeVBLongLongValue()
    {
        var initialValue = 42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        context.Is64BitHost = true;

        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBLongPtrType.TypeInfo, "TEST");
        var value = new VBLongPtrValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);
        
        var sut = CreateNegationOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBLongLongValue, result.TypeInfo.Name);
        Assert.AreEqual(-initialValue, ((VBLongLongValue)result).Value);
        Console.WriteLine(((VBLongLongValue)result).Value);
    }

    [TestMethod]
    public void EvaluateResultGivenPositiveNumericValue_ReturnsSameNumericType_VBInteger()
    {
        var initialValue = 42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBIntegerType.TypeInfo, "TEST");
        var value = new VBIntegerValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateNegationOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBIntegerValue);
    }

    [TestMethod]
    public void EvaluateResultGivenPositiveNumericValue_ReturnsSameNumericType_VBLong()
    {
        var initialValue = 42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBLongType.TypeInfo, "TEST");
        var value = new VBLongValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateNegationOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBLongValue);
    }

    [TestMethod]
    public void EvaluateResultGivenPositiveNumericValue_ReturnsSameNumericType_VBLongLong()
    {
        var initialValue = 42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBLongLongType.TypeInfo, "TEST");
        var value = new VBLongLongValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateNegationOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBLongLongValue);
    }

    [TestMethod]
    public void EvaluateResultGivenPositiveNumericValue_ReturnsSameNumericType_VBCurrency()
    {
        var initialValue = 42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBCurrencyType.TypeInfo, "TEST");
        var value = new VBCurrencyValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateNegationOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBCurrencyValue);
    }

    [TestMethod]
    public void EvaluateResultGivenPositiveNumericValue_ReturnsSameNumericType_VBDecimal()
    {
        var initialValue = 42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBDecimalType.TypeInfo, "TEST");
        var value = new VBDecimalValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateNegationOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBDecimalValue);
    }

    [TestMethod]
    public void EvaluateResultGivenPositiveNumericValue_ReturnsSameNumericType_VBSingle()
    {
        var initialValue = 42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBSingleType.TypeInfo, "TEST");
        var value = new VBSingleValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateNegationOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBSingleValue);
    }

    [TestMethod]
    public void EvaluateResultGivenPositiveNumericValue_ReturnsSameNumericType_VBDouble()
    {
        var initialValue = 42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBDoubleType.TypeInfo, "TEST");
        var value = new VBDoubleValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateNegationOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBDoubleValue);
    }


    [TestMethod]
    public void EvaluateResultGivenNegativeVBLongPtrValue_32bit_ReturnsPositiveVBLongValue()
    {
        var initialValue = -42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        context.Is64BitHost = false;

        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBLongPtrType.TypeInfo, "TEST");
        var value = new VBLongPtrValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateNegationOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBLongValue, result.TypeInfo.Name);
        Assert.AreEqual(-initialValue, ((VBLongValue)result).Value);
        Console.WriteLine(((VBLongValue)result).Value);
    }

    [TestMethod]
    public void EvaluateResultGivenNegativeVBLongPtrValue_64bit_ReturnsPositiveVBLongLongValue()
    {
        var initialValue = -42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        context.Is64BitHost = true;

        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBLongPtrType.TypeInfo, "TEST");
        var value = new VBLongPtrValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateNegationOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBLongLongValue, result.TypeInfo.Name);
        Assert.AreEqual(-initialValue, ((VBLongLongValue)result).Value);
        Console.WriteLine(((VBLongLongValue)result).Value);
    }

    [TestMethod]
    public void EvaluateResultGivenNegativeNumericValue_ReturnsSameNumericType_VBInteger()
    {
        var initialValue = -42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBIntegerType.TypeInfo, "TEST");
        var value = new VBIntegerValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateNegationOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBIntegerValue);
    }

    [TestMethod]
    public void EvaluateResultGivenNegativeNumericValue_ReturnsSameNumericType_VBLong()
    {
        var initialValue = -42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBLongType.TypeInfo, "TEST");
        var value = new VBLongValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateNegationOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBLongValue);
    }

    [TestMethod]
    public void EvaluateResultGivenNegativeNumericValue_ReturnsSameNumericType_VBLongLong()
    {
        var initialValue = -42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBLongLongType.TypeInfo, "TEST");
        var value = new VBLongLongValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateNegationOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBLongLongValue);
    }

    [TestMethod]
    public void EvaluateResultGivenNegativeNumericValue_ReturnsSameNumericType_VBCurrency()
    {
        var initialValue = -42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBCurrencyType.TypeInfo, "TEST");
        var value = new VBCurrencyValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateNegationOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBCurrencyValue);
    }

    [TestMethod]
    public void EvaluateResultGivenNegativeNumericValue_ReturnsSameNumericType_VBDecimal()
    {
        var initialValue = -42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBDecimalType.TypeInfo, "TEST");
        var value = new VBDecimalValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateNegationOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBDecimalValue);
    }

    [TestMethod]
    public void EvaluateResultGivenNegativeNumericValue_ReturnsSameNumericType_VBSingle()
    {
        var initialValue = -42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBSingleType.TypeInfo, "TEST");
        var value = new VBSingleValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateNegationOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBSingleValue);
    }

    [TestMethod]
    public void EvaluateResultGivenNegativeNumericValue_ReturnsSameNumericType_VBDouble()
    {
        var initialValue = -42;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBDoubleType.TypeInfo, "TEST");
        var value = new VBDoubleValue(variable) { NumericValue = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateNegationOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBDoubleValue);
    }

    [TestMethod]
    public void EvaluateResultGivenNothingObjectValue_ThrowsError91()
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBObjectType.TypeInfo, "TEST");

        var sut = CreateNegationOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);

        Assert.IsNull(result);
        Assert.IsTrue(scope.ActiveErrorState);
        Assert.AreEqual(VBRuntimeErrorException.ObjectVariableNotSet(variable).VBErrorNumber, scope.Error?.VBErrorNumber);
    }

    [TestMethod]
    public void EvaluateResultGivenObjectValueWithoutDefaultMember_ThrowsError438()
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBObjectType.TypeInfo, "TEST");
        
        var value = new VBObjectValue(variable) { Value = Guid.NewGuid() };
        context.SetSymbolValue(variable, value);

        var sut = CreateNegationOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);

        Assert.IsNull(result);
        Assert.IsTrue(scope.ActiveErrorState);
        Assert.AreEqual(VBRuntimeErrorException.ObjectDoesntSupportPropertyOrMethod(variable).VBErrorNumber, scope.Error?.VBErrorNumber);
    }

    [TestMethod]
    public void EvaluateResultGivenVBEmptyValue_ReturnsVBLongZero()
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBVariantType.TypeInfo, "TEST");

        var sut = CreateNegationOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.AreEqual(VBLongValue.Zero, result);
    }

    [TestMethod]
    public void EvaluateResultGivenVBDateValue_ReturnsVBDateValue()
    {
        var initialValue = DateTime.Now;

        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBDateType.TypeInfo, "TEST");
        var value = new VBDateValue(variable) { Value = initialValue };
        context.SetSymbolValue(variable, value);

        var sut = CreateNegationOperator(ref procedure, variable);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);

        Assert.IsNull(scope.Error, scope.Error?.ToString());
        Assert.IsTrue(result is VBDateValue, result?.TypeInfo.Name);
    }
}
