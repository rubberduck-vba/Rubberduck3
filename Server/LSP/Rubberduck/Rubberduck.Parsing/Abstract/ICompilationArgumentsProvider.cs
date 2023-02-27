using Rubberduck.Parsing.PreProcessing;
using System.Collections.Generic;

namespace Rubberduck.Parsing.Abstract
{
    public interface ICompilationArgumentsProvider
    {
        VBAPredefinedCompilationConstants PredefinedCompilationConstants { get; }
        Dictionary<string, short> UserDefinedCompilationArguments(string projectId);
    }
}
