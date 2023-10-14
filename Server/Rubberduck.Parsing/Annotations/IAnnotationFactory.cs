using Rubberduck.Parsing.Grammar;
using Rubberduck.Unmanaged.Model;

namespace Rubberduck.Parsing.Annotations;

public interface IAnnotationFactory
{
    IParseTreeAnnotation Create(VBAParser.AnnotationContext context, QualifiedSelection qualifiedSelection);
}
