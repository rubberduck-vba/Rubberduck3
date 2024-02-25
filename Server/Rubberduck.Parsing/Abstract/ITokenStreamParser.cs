using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Abstract;

public interface ITokenStreamParser
{
    IParseTree Parse(WorkspaceFileUri uri, CommonTokenStream tokenStream, CancellationToken token, out IEnumerable<SyntaxErrorInfo> errors, ParserMode parserMode = ParserMode.FallBackSllToLl, IEnumerable<IParseTreeListener>? parseListeners = null);
}
