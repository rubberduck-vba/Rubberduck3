using Antlr4.Runtime;
using System.IO;

namespace Rubberduck.Parsing.Abstract
{
    public interface ICommonTokenStreamProvider<TContent>
    {
        CommonTokenStream Tokens(TContent content);
    }
}
