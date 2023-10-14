using System;
using Antlr4.Runtime;

namespace Rubberduck.Parsing.Abstract
{
    public interface IRubberduckParseErrorListener : IParserErrorListener
    {
        bool HasPostponedException(out Exception exception);
    }
}
