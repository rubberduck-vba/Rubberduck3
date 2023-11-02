// ReSharper disable once CheckNamespace
namespace Rubberduck.Parsing.Grammar.PartialExtensions;

public interface IAnnotatedContext
{
    //Attributes Attributes { get; }
    IEnumerable<VBAParser.AnnotationContext> Annotations { get; }

    /// <summary>
    /// The token index any missing attribute would be inserted at.
    /// </summary>
    int AttributeTokenIndex { get; }

    void Annotate(VBAParser.AnnotationContext annotation);
    //void AddAttributes(Attributes attributes);
}