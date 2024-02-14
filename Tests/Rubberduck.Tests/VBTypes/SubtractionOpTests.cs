using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Operators;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using System;
using Rubberduck.InternalApi.Extensions;

namespace Rubberduck.Tests.VBTypes;

[TestClass]
public class SubtractionOpTests : WideningArithmeticOpTests
{
    protected override VBBinaryOperator CreateOperator(WorkspaceUri uri, TypedSymbol lhs, TypedSymbol rhs) => new VBSubtractionOperator(uri, lhs.Name, rhs.Name, lhs, rhs);
    protected override double ExpectResult(double lhs, double rhs) => lhs - rhs;
    protected override T ExpectResult<T>(DateTime lhs, int rhs) => (T)Convert.ChangeType(DateTime.FromOADate(ExpectResult(lhs.ToOADate(), rhs)), typeof(T));

    [TestMethod]
    [TestCategory("Operators")]
    [DataRow(128, 255)]
    [DataRow(0, 1)]
    [DataRow(32, 64)]
    public virtual void GivenVBByteOperands_TotalOverflows_ThrowsOverflowError(int lhsValue, int rhsValue)
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
    [DataRow(-32750, 32750)]
    [DataRow(32767, -1)]
    public override void GivenVBIntegerOperands_OpOverflows_ThrowsOverflowError(int lhsValue, int rhsValue) =>
        base.GivenVBIntegerOperands_OpOverflows_ThrowsOverflowError(lhsValue, rhsValue);

}
