using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Rubberduck.Parsing.Grammar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.Parsing.Abstract
{
    public interface IParser<TContent>
    {
        ParserResult Parse(string moduleName, TContent content, Func<VBAParser, IParseTree> startRule, IEnumerable<IParseTreeListener> parseListeners, ParserMode mode = ParserMode.Default);
    }
}
