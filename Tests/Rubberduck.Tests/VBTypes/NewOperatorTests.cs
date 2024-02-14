using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model;
using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Operators;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using System;

namespace Rubberduck.Tests.VBTypes;


[TestClass]
public class NewOperatorTests : UnaryOperatorTests
{
    protected override VBUnaryOperator CreateOperator(WorkspaceUri uri, TypedSymbol symbol) =>
        new VBNewOperator(expression: $"{Tokens.New} {symbol.Name}", symbol, uri);

    [TestMethod]
    [TestCategory("Operators")]
    public void GivenClassType_ReturnsVBObjectValue()
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var classType = CreateClassType(context, "Class1");

        var sut = CreateOperator(ref procedure, classType);

        var scope = context.EnterScope(procedure);
        var result = sut.Evaluate(ref scope);
        OutputExecutionScope(scope);

        Assert.IsNotNull(result);
        Assert.IsTrue(result is VBObjectValue);
        Console.WriteLine(((VBObjectValue)result).Value);
    }

    [TestMethod]
    [TestCategory("Operators")]
    [ExpectedException(typeof(VBCompileErrorException))]
    public void GivenNonClassType_ThrowsCompileError()
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var variable = CreateVariable(context, VBIntegerType.TypeInfo, "Thing");

        var sut = CreateOperator(ref procedure, variable);
        var scope = context.EnterScope(procedure);

        try
        {
            sut.Evaluate(ref scope, rethrow: true);
        }
        catch (VBCompileErrorException e)
        {
            Assert.AreEqual(VBCompileErrorId.ExpectedIdentifier, e.VBCompileErrorId);
            throw;
        }
        finally
        {
            OutputExecutionScope(scope);
        }
    }

    [TestMethod]
    [TestCategory("Operators")]
    [ExpectedException(typeof(VBRuntimeErrorException))]
    public void GivenNonCreatableClassType_ThrowsError430()
    {
        var context = Services.GetRequiredService<VBExecutionContext>();
        var procedure = ParentProcedure;
        var classType = CreateClassType(context, "Class1", false, false);

        var sut = CreateOperator(ref procedure, classType);
        var scope = context.EnterScope(procedure);

        try
        {
            sut.Evaluate(ref scope, rethrow: true);
        }
        catch (VBRuntimeErrorException e)
        {
            Assert.AreEqual(430, e.VBErrorNumber);
            throw;
        }
        finally
        {
            OutputExecutionScope(scope);
        }
    }
}
