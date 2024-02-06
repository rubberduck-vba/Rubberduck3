using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public interface INumericValue
{
    VBType TypeInfo { get; }
    double AsDouble();
    int AsLong();
    short AsInteger();

    VBTypedValue WithValue(double value);
}

public interface INumericCoercion
{
    double? AsCoercedNumeric(int depth = 0);
}

public interface IStringCoercion
{
    string AsCoercedString(int depth = 0);
}
