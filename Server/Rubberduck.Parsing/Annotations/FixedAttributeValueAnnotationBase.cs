using Rubberduck.InternalApi.Model.Declarations;
using Rubberduck.Unmanaged.Model;

namespace Rubberduck.Parsing.Annotations;

public abstract class FixedAttributeValueAnnotationBase : AnnotationBase, IAttributeAnnotation
{
    private readonly string _attribute;
    private readonly IReadOnlyList<string> _attributeValues;

    protected FixedAttributeValueAnnotationBase(string name, AnnotationTarget target, string attribute, IEnumerable<string> attributeValues, bool allowMultiple = false, IReadOnlyList<ComponentType> incompatibleComponentTypes = null)
        : base(name, target, allowMultiple: allowMultiple)
    {
        // IEnumerable makes specifying the compile-time constant list easier on us
        _attributeValues = attributeValues.ToList();
        _attribute = attribute;
    }

    public override IReadOnlyList<ComponentKind> IncompatibleComponentKinds { get; } = [ComponentKind.DocumentModule];

    public IReadOnlyList<string> AnnotationToAttributeValues(IReadOnlyList<string> annotationValues) => _attributeValues;

    public string Attribute(IReadOnlyList<string> annotationValues) => _attribute;

    // annotation values must not be specified, because attribute values are fixed in the first place
    public IReadOnlyList<string> AttributeToAnnotationValues(IReadOnlyList<string> attributeValues) => [];

    public bool MatchesAttributeDefinition(string attributeName, IReadOnlyList<string> attributeValues) => 
        _attribute == attributeName && _attributeValues.SequenceEqual(attributeValues);
}