using Antlr4.Runtime;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Abstract;

public interface ITokenStreamPreprocessor
{
    CommonTokenStream? PreprocessTokenStream(WorkspaceFileUri uri, CommonTokenStream tokenStream, CancellationToken token);
}
