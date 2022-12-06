using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Rubberduck.Parsing;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Rubberduck.UI.Abstract
{
    public interface ICodeParserService
    {
        Task<ParserResult> ParseAsync(string moduleName, TextReader reader, IEnumerable<IParseTreeListener> parseListeners);
        ParserResult Parse(string moduleName, TextReader reader, IEnumerable<IParseTreeListener> parseListeners);
    }
}