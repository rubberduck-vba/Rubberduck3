using System.Threading;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Abstract
{
    public interface IStringParser
    {
        (IParseTree tree, ITokenStream tokenStream) Parse(string moduleName, string projectId, string code, CancellationToken token, CodeKind codeKind = CodeKind.RubberduckEditorModule, ParserMode parserMode = ParserMode.FallBackSllToLl);
    }
}
