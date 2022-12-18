using System.IO;
using System.Threading;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Abstract
{
    public interface IParser<TContent>
    {
        (IParseTree tree, ITokenStream tokenStream) Parse(string moduleName, string projectId, TContent content, CancellationToken token, CodeKind codeKind = CodeKind.RubberduckEditorModule, ParserMode parserMode = ParserMode.FallBackSllToLl);
    }

    public interface IStringParser : IParser<string> { }

    public interface ITextReaderParser : IParser<TextReader> { }
}
