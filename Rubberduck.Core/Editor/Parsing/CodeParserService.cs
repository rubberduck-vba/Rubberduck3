using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Rubberduck.Parsing;
using Rubberduck.Parsing.Abstract;
using Rubberduck.UI.Abstract;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rubberduck.Core.Editor
{
    public class CodeParserService : ICodeParserService
    {
        private readonly IParser<TextReader> _parser;

        public CodeParserService(IParser<TextReader> parser)
        {
            _parser = parser;
        }

        public async Task<ParserResult> ParseAsync(string moduleName, TextReader reader, IEnumerable<IParseTreeListener> parseListeners)
        {
            return await Task.Run(() => Parse(moduleName, reader, parseListeners));
        }

        public ParserResult Parse(string moduleName, TextReader reader, IEnumerable<IParseTreeListener> parseListeners)
        {
            return _parser.Parse(moduleName, reader, e => e.startRule(), parseListeners, ParserMode.Ll);
        }
    }
}