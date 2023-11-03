using Antlr4.Runtime;
using Rubberduck.Parsing.Expressions;
using System.Collections.Generic;

namespace Rubberduck.Parsing.Abstract
{
    public interface IValue
    {
        Expressions.ValueType ValueType { get; }
        bool AsBool { get; }
        byte AsByte { get; }
        decimal AsDecimal { get; }
        System.DateTime AsDate { get; }
        string AsString { get; }
        IEnumerable<IToken> AsTokens {get;}
    }
}
