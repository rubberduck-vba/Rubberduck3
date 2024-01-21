﻿using Rubberduck.InternalApi.Model.Declarations;
using System.Collections.Immutable;

namespace Rubberduck.Parsing.Annotations.Concrete;

/// <summary>
/// @ModuleAttribute annotation, indicates the presence of a hidden module-level attribute; allows specifying arbitrary VB_Attribute for modules. Use the quick-fixes to "Rubberduck Opportunities" code inspections to synchronize annotations and attributes.
/// </summary>
/// <parameter name="VB_Attribute" type="Identifier">
/// The literal identifier name of the member VB_Attribute.
/// </parameter>
/// <parameter name="Values" type="ParamArray">
/// The comma-separated attribute values, as applicable.
/// </parameter>
/// <example>
/// <before>
/// <module name="Class1" type="Class Module">
/// <![CDATA[
/// '@ModuleAttribute VB_Ext_Key, "Key", "Value"
/// Option Explicit
///
/// Public Sub DoSomething()
/// End Sub
/// ]]>
/// </module>
/// </before>
/// <after>
/// <module name="Class1" type="Class Module">
/// <![CDATA[
/// Attribute VB_Ext_KEY = "Key", "Value"
/// '@ModuleAttribute VB_Ext_Key, "Key", "Value"
/// Option Explicit
///
/// Public Sub DoSomething()
/// End Sub
/// ]]>
/// </module>
/// </after>
/// </example>
public class ModuleAttributeAnnotation : FlexibleAttributeAnnotationBase
{
    public ModuleAttributeAnnotation()
    : base("ModuleAttribute", AnnotationTarget.Module, _argumentTypes, true)
    {
        IncompatibleComponentKinds = new HashSet<ComponentKind>(base.IncompatibleComponentKinds)
        { 
            ComponentKind.DocumentModule
        }.ToImmutableList();
    }

    public override IReadOnlyList<ComponentKind> IncompatibleComponentKinds { get; }

    private static readonly AnnotationArgumentType[] _argumentTypes =
    [
        AnnotationArgumentType.Attribute,
        AnnotationArgumentType.Text | AnnotationArgumentType.Number | AnnotationArgumentType.Boolean
    ];
}