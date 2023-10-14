using Antlr4.Runtime;

namespace Rubberduck.Parsing.TokenStreamProviders;

public class StringTokenStreamProvider : CommonTokenStreamProvider<string>
{
    protected override AntlrInputStream GetInputStream(string content) => new AntlrInputStream(content);
}
