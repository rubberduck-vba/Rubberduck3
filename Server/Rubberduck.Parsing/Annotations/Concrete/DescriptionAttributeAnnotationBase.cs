using Rubberduck.InternalApi.Model.Declarations;

namespace Rubberduck.Parsing.Annotations.Concrete;

public abstract class DescriptionAttributeAnnotationBase : FlexibleAttributeValueAnnotationBase
{
    public DescriptionAttributeAnnotationBase(string name, AnnotationTarget target, string attribute)
        : base(name, target, attribute, 1, new List<AnnotationArgumentType> { AnnotationArgumentType.Text })
    {}
}