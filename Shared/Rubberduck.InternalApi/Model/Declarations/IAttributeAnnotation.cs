using System.Collections.Generic;

namespace Rubberduck.InternalApi.Model.Declarations;

public interface IAttributeAnnotation : IAnnotation
{
    bool MatchesAttributeDefinition(string attributeName, IReadOnlyList<string> attributeValues);
    string Attribute(IReadOnlyList<string> annotationValues);

    IReadOnlyList<string> AnnotationToAttributeValues(IReadOnlyList<string> annotationValues);
    IReadOnlyList<string> AttributeToAnnotationValues(IReadOnlyList<string> attributeValues);
}

public static class AttributeAnnotationExtensions
{
    public static string Attribute(this IAttributeAnnotation annotation, IParseTreeAnnotation annotationInstance)
    {
        return annotation.Attribute(annotationInstance.AnnotationArguments);
    }

    public static IReadOnlyList<string> AttributeValues(this IAttributeAnnotation annotation, IParseTreeAnnotation instance)
    {
        return annotation.AnnotationToAttributeValues(instance.AnnotationArguments);
    }
}
