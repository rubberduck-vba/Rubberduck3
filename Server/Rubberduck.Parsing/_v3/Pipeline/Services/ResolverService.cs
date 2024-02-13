using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;

namespace Rubberduck.Parsing._v3.Pipeline;

public class ResolverService : ServiceBase, IResolverService
{
    public ResolverService(ILogger<ResolverService> logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance) 
        : base(logger, settingsProvider, performance)
    {
    }

    public VBType? Resolve(TypedSymbol symbol)
    {
        if (symbol.ResolvedType != null)
        {
            return symbol.ResolvedType;
        }

        //var types = _context.MemberOwnerTypes;
        //// FIXME this is placeholder code right here
        //var typeSymbol = types.SingleOrDefault(e => string.Equals(e.Name, symbol.Name, StringComparison.InvariantCultureIgnoreCase));

        //if (typeSymbol != null)
        //{
        //    var resolvedSymbol = (TypedSymbol)symbol.WithResolvedType(typeSymbol.ResolvedType);
        //    _context.AddTypedSymbol(resolvedSymbol);
        //}
        //return typeSymbol?.ResolvedType;
        throw new NotImplementedException();
    }
}
