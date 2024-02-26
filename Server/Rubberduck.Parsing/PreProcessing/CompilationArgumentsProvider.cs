using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Services;
using Rubberduck.Parsing.Abstract;
using Rubberduck.Unmanaged.TypeLibs.Abstract;
using Rubberduck.Unmanaged.UIContext;

namespace Rubberduck.Parsing.PreProcessing;

// TODO move this client-side; to the language server these are just symbols injected into the execution context.

public class CompilationArgumentsProvider : ICompilationArgumentsProvider
{
    private readonly IUiDispatcher _uiDispatcher;
    private readonly ITypeLibWrapperProvider _typeLibWrapperProvider;

    public CompilationArgumentsProvider(ITypeLibWrapperProvider typeLibWrapperProvider, IUiDispatcher uiDispatcher, VBAPredefinedCompilationConstants predefinedConstants)
    {
        _typeLibWrapperProvider = typeLibWrapperProvider;
        _uiDispatcher = uiDispatcher;
        PredefinedCompilationConstants = predefinedConstants;
    }

    public VBAPredefinedCompilationConstants PredefinedCompilationConstants { get; }

    public Dictionary<string, short> UserDefinedCompilationArguments(WorkspaceUri uri)
    {
        return GetUserDefinedCompilationArguments(uri);
    }

    private Dictionary<string, short> GetUserDefinedCompilationArguments(Uri uri)
    {
        // use the TypeLib API to grab the user defined compilation arguments; must be obtained on the main thread.
        var task = _uiDispatcher.StartTask(() => {
            using var typeLib = _typeLibWrapperProvider.TypeLibWrapperFromProject(uri);
            return typeLib?.VBEExtensions.ConditionalCompilationArguments ?? new Dictionary<string, short>();
        });
        return task.Result;
    }
}


public class WorkspaceCompilationArgumentsProvider : ICompilationArgumentsProvider
{
    private readonly IWorkspaceStateManager _state;

    public WorkspaceCompilationArgumentsProvider(IWorkspaceStateManager state)
    {
        _state = state;
    }

    public VBAPredefinedCompilationConstants PredefinedCompilationConstants => 
        new(_state.ActiveWorkspace?.ExecutionContext.LanguageVersion ?? throw new InvalidOperationException("Operation requires an active workspace"));

    public Dictionary<string, short> UserDefinedCompilationArguments(WorkspaceUri workspaceRoot)
    {
        var context = _state.GetWorkspace(workspaceRoot).ExecutionContext;
        return context.ResolvedSymbols.OfType<PrecompilerConstantSymbol>()
            .ToDictionary(symbol => symbol.Name, symbol => ((INumericValue)context.GetSymbolValue(symbol)).AsInteger().Value);
    }
}