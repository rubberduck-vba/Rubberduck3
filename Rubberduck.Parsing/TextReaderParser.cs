using Antlr4.Runtime;
using Rubberduck.Parsing.Abstract;
using System.Collections;
using System.IO;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography.X509Certificates;

namespace Rubberduck.Parsing
{
    public class TextReaderParser : Parser<TextReader>
    {
        public TextReaderParser(ICommonTokenStreamProvider<TextReader> provider) : base(provider) { }
        protected override CommonTokenStream GetCommonTokenStream(ICommonTokenStreamProvider<TextReader> provider, TextReader content) => provider.Tokens(content);
    }
}
