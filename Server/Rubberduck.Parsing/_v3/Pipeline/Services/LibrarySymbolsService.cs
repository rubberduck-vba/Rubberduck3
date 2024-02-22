using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing.COM.Abstract;
using Rubberduck.Parsing.Model.ComReflection;

namespace Rubberduck.Parsing._v3.Pipeline;

public class LibrarySymbolsService : ServiceBase
{
    private readonly IComLibraryProvider _comLibraryProvider;

    public LibrarySymbolsService(IComLibraryProvider comLibraryProvider,
        ILogger<LibrarySymbolsService> logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance) 
        : base(logger, settingsProvider, performance)
    {
        _comLibraryProvider = comLibraryProvider;
    }

    public ProjectSymbol LoadSymbolsFromTypeLibrary(Reference reference)
    {
        var uri = reference.Uri;
        if (string.IsNullOrWhiteSpace(uri))
        {
            throw new InvalidOperationException("Uri cannot be null or empty.");
        }

        var typeLib = _comLibraryProvider.LoadTypeLibrary(uri);
        if (typeLib is null)
        {
            throw new InvalidOperationException($"Could not load referenced type library '{reference.Name}' ({reference.Uri}).");
        }

        var project = new ComProject(typeLib, uri);
        return (ProjectSymbol)project.ToSymbol();
    }
}