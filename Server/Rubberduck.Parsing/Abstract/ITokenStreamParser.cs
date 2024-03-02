using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Exceptions;

namespace Rubberduck.Parsing.Abstract;

public interface ITokenStreamParser
{
    IParseTree Parse(WorkspaceFileUri uri, CommonTokenStream tokenStream, CancellationToken token, 
        out IEnumerable<SyntaxErrorException> errors, 
        out IEnumerable<Diagnostic> diagnostics,
        ParserMode parserMode = ParserMode.FallBackSllToLl, 
        IEnumerable<IParseTreeListener>? parseListeners = null);
}
