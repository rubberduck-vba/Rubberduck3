using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Abstract;

public interface ITokenStreamParser
{
    IParseTree Parse(WorkspaceFileUri uri, CommonTokenStream tokenStream, CancellationToken token, CodeKind codeKind = CodeKind.RubberduckEditorModule, ParserMode parserMode = ParserMode.FallBackSllToLl, IEnumerable<IParseTreeListener>? parseListeners = null);
}
