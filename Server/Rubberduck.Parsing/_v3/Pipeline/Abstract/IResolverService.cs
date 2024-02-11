using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.Parsing._v3.Pipeline;

public interface IResolverService
{
    VBType? Resolve(TypedSymbol symbol);
}
