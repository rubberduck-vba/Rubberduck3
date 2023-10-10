using Antlr4.Runtime.Misc;

// ReSharper disable once CheckNamespace
namespace Rubberduck.Parsing.Grammar.PartialExtensions
{
    public interface IIdentifierContext
    {
        Interval IdentifierTokens { get; }
    }
}
