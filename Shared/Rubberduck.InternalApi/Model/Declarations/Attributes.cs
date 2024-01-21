using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations;

public record class AttributeNode : IEquatable<AttributeNode>
{
    public AttributeNode(string attributeName, IEnumerable<string> values)
    {
        Name = attributeName;
        Values = values.ToList();
    }

    public string Name { get; }

    //The order of the values matters for some attributes, e.g. VB_Ext_Key.
    public IReadOnlyList<string> Values { get; }

    public bool HasValue(string value)
    {
        return Values.Any(item => item.Equals(value, StringComparison.OrdinalIgnoreCase));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name.ToUpperInvariant());
    }
}

/// <summary>
/// A dictionary storing values for a given attribute.
/// </summary>
/// <remarks>
/// Dictionary key is the attribute name/identifier.
/// </remarks>
public class Attributes : HashSet<AttributeNode>
{
    public static bool IsDefaultAttribute(ComponentKind componentType, string attributeName, IReadOnlyList<string> attributeValues)
    {
        if (!ComponentsWithDefaultAttributes.Contains(componentType))
        {
            return false;
        }

        return attributeName switch
        {
            "VB_Name" => true,
            "VB_GlobalNameSpace" => (componentType == ComponentKind.ClassModule || componentType == ComponentKind.UserFormModule)
                                   && attributeValues[0].Equals(Tokens.False),
            "VB_Exposed" => (componentType == ComponentKind.ClassModule || componentType == ComponentKind.UserFormModule)
                                   && attributeValues[0].Equals(Tokens.False),
            "VB_Creatable" => (componentType == ComponentKind.ClassModule || componentType == ComponentKind.UserFormModule)
                                   && attributeValues[0].Equals(Tokens.False),
            "VB_PredeclaredId" => (componentType == ComponentKind.ClassModule && attributeValues[0].Equals(Tokens.False))
                                   || (componentType == ComponentKind.UserFormModule && attributeValues[0].Equals(Tokens.True)),
            _ => false,
        };
    }

    private static readonly HashSet<ComponentKind> ComponentsWithDefaultAttributes =
    [
        ComponentKind.StandardModule,
        ComponentKind.ClassModule,
        ComponentKind.UserFormModule,
    ];

    public static string AttributeBaseName(string attributeName, string memberName)
    {
        return attributeName.StartsWith($"{memberName}.")
            ? attributeName[(memberName.Length + 1)..]
            : attributeName;
    }

    public static string MemberAttributeName(string attributeBaseName, string memberName)
    {
        return $"{memberName}.{attributeBaseName}";
    }

    public bool HasAttributeFor(IParseTreeAnnotation annotation, string? memberName = null)
    {
        return AttributeNodesFor(annotation, memberName).Any();
    }

    public IEnumerable<AttributeNode> AttributeNodesFor(IParseTreeAnnotation annotationInstance, string? memberName = null)
    {
        if (annotationInstance.Annotation is not IAttributeAnnotation annotation)
        {
            return Enumerable.Empty<AttributeNode>();
        }
        var attribute = annotation.Attribute(annotationInstance);

        var attributeName = memberName != null
            ? MemberAttributeName(attribute, memberName)
            : attribute;
        //VB_Ext_Key annotation depend on the defined key for identity.
        if (attribute.Equals("VB_Ext_Key", StringComparison.OrdinalIgnoreCase))
        {
            return this.Where(a => a.Name.Equals(attributeName, StringComparison.OrdinalIgnoreCase)
                                 && a.Values[0] == annotation.AttributeValues(annotationInstance)[0]);
        }

        return this.Where(a => a.Name.Equals(attributeName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Specifies that a member is the type's default member.
    /// Only one member in the type is allowed to have this attribute.
    /// </summary>
    /// <param name="identifierName"></param>
    public void AddDefaultMemberAttribute(string identifierName)
    {
        Add(new AttributeNode(identifierName + ".VB_UserMemId", ["0"]));
    }

    public bool HasDefaultMemberAttribute(string identifierName, out AttributeNode attribute)
    {
        attribute = this.SingleOrDefault(a => a.HasValue("0") 
            && a.Name.Equals($"{identifierName}.VB_UserMemId", StringComparison.OrdinalIgnoreCase))!;
        return attribute != null;
    }

    public bool HasDefaultMemberAttribute()
    {
        return this.Any(a => a.HasValue("0") 
            && a.Name.EndsWith(".VB_UserMemId", StringComparison.OrdinalIgnoreCase));
    }

    public void AddHiddenMemberAttribute(string identifierName)
    {
        Add(new AttributeNode(identifierName + ".VB_MemberFlags", ["40"]));
    }

    public bool HasHiddenMemberAttribute(string identifierName, out AttributeNode attribute)
    {
        attribute = this.SingleOrDefault(a => a.HasValue("40")
            && a.Name.Equals($"{identifierName}.VB_MemberFlags", StringComparison.OrdinalIgnoreCase))!;
        return attribute != null;
    }

    public void AddEnumeratorMemberAttribute(string identifierName)
    {
        Add(new AttributeNode(identifierName + ".VB_UserMemId", ["-4"]));
    }

    /// <summary>
    /// Corresponds to DISPID_EVALUATE
    /// </summary>
    /// <param name="identifierName"></param>
    public void AddEvaluateMemberAttribute(string identifierName)
    {
        Add(new AttributeNode(identifierName + ".VB_UserMemId", ["-5"]));
    }

    public bool HasEvaluateMemberAttribute(string identifierName, out AttributeNode attribute)
    {
        attribute = this.SingleOrDefault(a => a.HasValue("-5")
                                              && a.Name.Equals($"{identifierName}.VB_UserMemId", StringComparison.OrdinalIgnoreCase))!;
        return attribute != null;
    }

    public void AddMemberDescriptionAttribute(string identifierName, string description)
    {
        Add(new AttributeNode(identifierName + ".VB_Description", new[] {description}));
    }

    public bool HasMemberDescriptionAttribute(string identifierName, out AttributeNode attribute)
    {
        attribute = this.SingleOrDefault(a => a.Name.Equals($"{identifierName}.VB_Description", StringComparison.OrdinalIgnoreCase))!;
        return attribute != null;
    }

    /// <summary>
    /// Corresponds to TYPEFLAGS.TYPEFLAG_FPREDECLID
    /// </summary>
    public void AddPredeclaredIdTypeAttribute()
    {
        Add(new AttributeNode("VB_PredeclaredId", new[] {Tokens.True}));
    }

    public AttributeNode? PredeclaredIdAttribute
    {
        get
        {
            return this.SingleOrDefault(a => a.Name.Equals("VB_PredeclaredId", StringComparison.OrdinalIgnoreCase));
        }
    }

    public AttributeNode? ExposedAttribute
    {
        get
        {
            return this.SingleOrDefault(a => a.Name.Equals("VB_Exposed", StringComparison.OrdinalIgnoreCase));
        }
    }

    public bool HasPredeclaredIdAttribute(out AttributeNode attribute)
    {
        attribute = this.SingleOrDefault(a => a.Name.Equals("VB_PredeclaredId", StringComparison.OrdinalIgnoreCase) && a.HasValue(Tokens.True))!;
        return attribute != null;
    }

    /// <summary>
    /// Corresponds to TYPEFLAG_FAPPOBJECT?
    /// </summary>
    public void AddGlobalClassAttribute()
    {
        Add(new AttributeNode("VB_GlobalNamespace", new[] {Tokens.True}));
    }

    public bool HasGlobalAttribute(out AttributeNode attribute)
    {
        attribute = this.SingleOrDefault(a => a.Name.Equals("VB_GlobalNamespace", StringComparison.OrdinalIgnoreCase) && a.HasValue(Tokens.True))!;
        return attribute != null;
    }

    public void AddExposedClassAttribute()
    {
        Add(new AttributeNode("VB_Exposed", new[] { Tokens.True }));
    }

    public bool HasExposedAttribute(out AttributeNode attribute)
    {
        attribute = this.SingleOrDefault(a => a.Name.Equals("VB_Exposed", StringComparison.OrdinalIgnoreCase) && a.HasValue(Tokens.True))!;
        return attribute != null;
    }

    /// <summary>
    /// Corresponds to *not* having the TYPEFLAG_FNONEXTENSIBLE flag set (which is the default for VBA).
    /// </summary>
    public void AddExtensibleClassAttribute()
    {
        Add(new AttributeNode("VB_Customizable", new[] { Tokens.True }));
    }

    public bool HasExtensibleAttribute(out AttributeNode attribute)
    {
        attribute = this.SingleOrDefault(a => a.Name.Equals("VB_Customizable", StringComparison.OrdinalIgnoreCase) && a.HasValue(Tokens.True))!;
        return attribute != null;
    }

    public bool HasAttribute(string attribute)
    {
        return this.Any(attributeNode => attributeNode.Name.Equals(attribute, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<AttributeNode> AttributeNodes(string attribute)
    {
        return this.Where(attributeNode => attributeNode.Name.Equals(attribute, StringComparison.OrdinalIgnoreCase));
    }
}
