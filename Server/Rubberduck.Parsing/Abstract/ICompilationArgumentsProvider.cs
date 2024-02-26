using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.PreProcessing;

namespace Rubberduck.Parsing.Abstract;

public interface ICompilationArgumentsProvider
{
    VBAPredefinedCompilationConstants PredefinedCompilationConstants { get; }
    Dictionary<string, short> UserDefinedCompilationArguments(WorkspaceUri workspaceRoot);
}
