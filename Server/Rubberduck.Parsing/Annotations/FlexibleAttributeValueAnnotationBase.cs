using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations;

namespace Rubberduck.Parsing.Annotations;

public abstract class FlexibleAttributeValueAnnotationBase : AnnotationBase, IAttributeAnnotation
{
    private readonly string _attribute;
    private readonly int _numberOfValues;

    protected FlexibleAttributeValueAnnotationBase(string name, AnnotationTarget target, string attribute, int numberOfValues, IReadOnlyList<AnnotationArgumentType> argumentTypes)
        : base(name, target, numberOfValues, numberOfValues, argumentTypes)
    {
        _attribute = attribute;
        _numberOfValues = numberOfValues;
    }

    public override IReadOnlyList<ComponentKind> IncompatibleComponentKinds { get; } = [ComponentKind.DocumentModule];

    public bool MatchesAttributeDefinition(string attributeName, IReadOnlyList<string> attributeValues) => 
        _attribute == attributeName && _numberOfValues == attributeValues.Count;

    public virtual IReadOnlyList<string> AnnotationToAttributeValues(IReadOnlyList<string> annotationValues) => 
        annotationValues.Take(_numberOfValues).Select(v => v.EnQuote()).ToList();

    public virtual IReadOnlyList<string> AttributeToAnnotationValues(IReadOnlyList<string> attributeValues) => 
        attributeValues.Take(_numberOfValues).Select(v => v.EnQuote()).ToList();

    public string Attribute(IReadOnlyList<string> annotationValues) => _attribute;
}