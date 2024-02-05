using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public interface INumericValue
{
    VBType TypeInfo { get; }
    double AsDouble();

    VBTypedValue WithValue(double value);
}

public interface INumericCoercion
{
    double? AsCoercedNumeric(int depth = 0);
}

public interface IStringCoercion
{
    string? AsCoercedString(int depth = 0);
}
