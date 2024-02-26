using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;

namespace Rubberduck.Parsing._v3.Pipeline;

public class ResolverService : ServiceBase, IResolverService
{
    private readonly IWorkspaceStateManager _workspaces;

    public ResolverService(IWorkspaceStateManager workspaces,
        ILogger<ResolverService> logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance) 
        : base(logger, settingsProvider, performance)
    {
        _workspaces = workspaces;
    }

    public VBType? Resolve(TypedSymbol symbol)
    {
        if (symbol.ResolvedType != null)
        {
            return symbol.ResolvedType;
        }

        var workspace = _workspaces.GetWorkspace(symbol.Uri.WorkspaceRoot);
        var root = workspace.WorkspaceRoot!;

        var context = workspace.ExecutionContext;

        if (symbol is DeclarationExpressionSymbol expression && !string.IsNullOrWhiteSpace(expression.TypeName))
        {
            var type = context.ResolveType(expression.TypeName, root);
            return type;
        }
        else if (symbol is FunctionSymbol function && !string.IsNullOrWhiteSpace(function.TypeName))
        {
            var type = context.ResolveType(function.TypeName, root);
            return type;
        }

        return VBVariantType.TypeInfo;
    }
}
