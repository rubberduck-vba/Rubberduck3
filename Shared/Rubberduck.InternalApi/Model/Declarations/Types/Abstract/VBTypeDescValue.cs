using Rubberduck.InternalApi.Model.Declarations.Execution.Values;

namespace Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

/// <summary>
/// A metatype that describes a type. Not used in many places!
/// </summary>
public record class VBTypeDescValue : VBTypedValue
{
    public VBTypeDescValue(VBType type) : base(type, null)
    {
    }

    public override int Size => sizeof(int);
}
