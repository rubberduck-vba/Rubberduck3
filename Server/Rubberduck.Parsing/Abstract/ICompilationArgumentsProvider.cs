using Rubberduck.Parsing.PreProcessing;

namespace Rubberduck.Parsing.Abstract;

public interface ICompilationArgumentsProvider
{
    VBAPredefinedCompilationConstants PredefinedCompilationConstants { get; }
    Dictionary<string, short> UserDefinedCompilationArguments(string projectId);
}
