using Antlr4.Runtime;

namespace Rubberduck.Parsing.Abstract
{
    public interface ICommonTokenStreamProvider<TContent>
    {
        CommonTokenStream Tokens(TContent content);
    }
}
