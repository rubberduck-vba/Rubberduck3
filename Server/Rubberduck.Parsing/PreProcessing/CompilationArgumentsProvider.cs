﻿using Rubberduck.Parsing.Abstract;
using Rubberduck.Unmanaged.TypeLibs.Abstract;
using Rubberduck.Unmanaged.UIContext;

namespace Rubberduck.Parsing.PreProcessing;

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

    public Dictionary<string, short> UserDefinedCompilationArguments(Uri uri)
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
