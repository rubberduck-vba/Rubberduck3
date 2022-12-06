using Antlr4.Runtime;
using System.IO;

namespace Rubberduck.Parsing
{
    public class TextReaderCommonTokenStreamProvider : CommonTokenStreamProvider<TextReader>
    {
        protected override AntlrInputStream GetInputStream(TextReader content) => new AntlrInputStream(content);
    }
}
