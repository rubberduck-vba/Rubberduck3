using Antlr4.Runtime;
using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.Grammar;

namespace Rubberduck.Parsing
{
    public abstract class CommonTokenStreamProvider<T> : ICommonTokenStreamProvider<T>
    {
        protected abstract AntlrInputStream GetInputStream(T content);

        public CommonTokenStream Tokens(T content)
        {
            var stream = GetInputStream(content);
            return Tokenize(stream);
        }

        private CommonTokenStream Tokenize(AntlrInputStream stream)
        {
            var lexer = new VBALexer(stream);
            return new CommonTokenStream(lexer);
        }
    }
}
