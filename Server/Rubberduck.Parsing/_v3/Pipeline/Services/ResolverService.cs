using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using ExecutionContext = Rubberduck.InternalApi.Model.Declarations.Execution.ExecutionContext;

namespace Rubberduck.Parsing._v3.Pipeline;

public class ResolverService : ServiceBase, IResolverService
{
    private readonly ExecutionContext _context;
    public ResolverService(ILogger logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance, ExecutionContext context) 
        : base(logger, settingsProvider, performance)
    {
        _context = context;
    }

    public VBType? Resolve(TypedSymbol symbol)
    {
        if (symbol.ResolvedType != null)
        {
            return symbol.ResolvedType;
        }

        var types = _context.MemberOwnerTypes;
        // FIXME this is placeholder code right here
        var typeSymbol = types.SingleOrDefault(e => string.Equals(e.Name, symbol.Name, StringComparison.InvariantCultureIgnoreCase));

        if (typeSymbol != null)
        {
            var resolvedSymbol = (TypedSymbol)symbol.WithResolvedType(typeSymbol.ResolvedType);
            _context.AddTypedSymbol(resolvedSymbol);
        }
        return typeSymbol?.ResolvedType;
    }
}
