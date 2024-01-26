using Rubberduck.InternalApi.Model.Declarations.Symbols;

namespace Rubberduck.InternalApi.Model.Declarations.Types.Abstract
{
    public interface IVBDeclaredType
    {
        Symbol Declaration { get; init; }
        Symbol[]? Definitions { get; init; }
    }
}
