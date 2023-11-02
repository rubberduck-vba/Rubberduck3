using Antlr4.Runtime;

namespace Rubberduck.Parsing.TokenStreamProviders;

public class TextReaderTokenStreamProvider : CommonTokenStreamProvider<TextReader>
{
    protected override AntlrInputStream GetInputStream(TextReader content) => new(content);
}
