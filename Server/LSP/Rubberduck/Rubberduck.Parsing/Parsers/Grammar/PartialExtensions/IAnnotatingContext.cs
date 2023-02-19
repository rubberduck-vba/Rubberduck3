using Antlr4.Runtime;

// ReSharper disable once CheckNamespace
namespace Rubberduck.Parsing.Grammar.PartialExtensions
{
    public interface IAnnotatingContext
    {
        ParserRuleContext AnnotatedContext { get; }
        string AnnotationType { get; }
    }
}