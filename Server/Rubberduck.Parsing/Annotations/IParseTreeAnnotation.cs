using Rubberduck.Parsing.Grammar;
using Rubberduck.Unmanaged.Model;

namespace Rubberduck.Parsing.Annotations;

public interface IParseTreeAnnotation
{
    // needs to be accessible to all external consumers
    IAnnotation Annotation { get; }
    IReadOnlyList<string> AnnotationArguments { get; }

    // needs to be accessible to IllegalAnnotationInspection
    int? AnnotatedLine { get; }
    VBAParser.AnnotationContext Context { get; }
    QualifiedSelection QualifiedSelection { get; }
}