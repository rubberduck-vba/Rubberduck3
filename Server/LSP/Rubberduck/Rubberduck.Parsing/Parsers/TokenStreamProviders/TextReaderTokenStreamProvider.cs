using Antlr4.Runtime;
using System.IO;

namespace Rubberduck.Parsing.TokenStreamProviders
{
    public class TextReaderTokenStreamProvider : CommonTokenStreamProvider<TextReader>
    {
        protected override AntlrInputStream GetInputStream(TextReader content) => new AntlrInputStream(content);
    }
}
