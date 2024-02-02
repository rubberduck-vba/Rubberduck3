using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Linq;
using System.Reflection;

namespace Rubberduck.InternalApi.ServerPlatform.LanguageServer;

public enum RubberduckSymbolKind
{
    /// <summary>
    /// This should be the default/fallback value.
    /// </summary>
    UnknownSymbol = 0,

    /* LSP: values 1-18 (from the initial LSP version) deemed supported if DocumentSymbolClientCapabilities.SymbolKind is not specified. */

    //File = SymbolKind.File, /* files and modules are interchangeable in Classic-VB; a single class/module is defined per file. */
    Module = SymbolKind.Module,
    //Namespace = SymbolKind.Namespace, /* no namespaces in Classic-VB */
    Project = SymbolKind.Package,
    Class = SymbolKind.Class,
    Procedure = SymbolKind.Method,
    Property = SymbolKind.Property,
    Field = SymbolKind.Field,
    //Constructor = SymbolKind.Constructor, /* no constructors in Classic-VB */
    Enum = SymbolKind.Enum,
    Interface = SymbolKind.Interface,
    Function = SymbolKind.Function,
    Variable = SymbolKind.Variable,
    Constant = SymbolKind.Constant,
    StringLiteral = SymbolKind.String,
    NumberLiteral = SymbolKind.Number,
    BooleanLiteral = SymbolKind.Boolean,
    Array = SymbolKind.Array,

    /* LSP: values 19-26 reserved v3.17 */

    Object = SymbolKind.Object,
    Key = SymbolKind.Key,
    Nothing = SymbolKind.Null,
    EnumMember = SymbolKind.EnumMember,
    UserDefinedType = SymbolKind.Struct,
    Event = SymbolKind.Event,
    Operator = SymbolKind.Operator,
    //TypeParameter = SymbolKind.TypeParameter, /* no generics in Classic-VB */
}

public static class RubberduckSymbolKindExtensions
{
    public static SymbolKind SymbolKind(this RubberduckSymbolKind value) => (SymbolKind)value;
}

public readonly struct RubberduckSemanticTokenType
{
    public static SemanticTokenType[] SemanticTokenTypes { get; } = typeof(RubberduckSemanticTokenType)
        .GetProperties(BindingFlags.Public | BindingFlags.Static)
        .Select(property => property.GetGetMethod())
        .Where(method => method != null && method.ReturnType == typeof(SemanticTokenType))
        .Select(method => (SemanticTokenType)method!.Invoke(null, null)!)
        .ToArray();

    public static SemanticTokenType Type { get; } = SemanticTokenType.Type;
    public static SemanticTokenType Class { get; } = SemanticTokenType.Class;
    public static SemanticTokenType Enum { get; } = SemanticTokenType.Enum;
    public static SemanticTokenType Interface { get; } = SemanticTokenType.Interface;
    public static SemanticTokenType UserDefinedType { get; } = SemanticTokenType.Struct;
    public static SemanticTokenType Parameter { get; } = SemanticTokenType.Parameter;
    public static SemanticTokenType Variable { get; } = SemanticTokenType.Variable;
    public static SemanticTokenType Property { get; } = SemanticTokenType.Property;
    public static SemanticTokenType EnumMember { get; } = SemanticTokenType.EnumMember;
    public static SemanticTokenType Event { get; } = SemanticTokenType.Event;
    public static SemanticTokenType Function { get; } = SemanticTokenType.Function;
    public static SemanticTokenType Procedure { get; } = SemanticTokenType.Method;
    public static SemanticTokenType Precompiler { get; } = SemanticTokenType.Macro;
    public static SemanticTokenType Keyword { get; } = SemanticTokenType.Keyword;
    public static SemanticTokenType Modifier { get; } = SemanticTokenType.Modifier;
    public static SemanticTokenType Comment { get; } = SemanticTokenType.Comment;
    public static SemanticTokenType StringLiteral { get; } = SemanticTokenType.String;
    public static SemanticTokenType NumberLiteral { get; } = SemanticTokenType.Number;
    public static SemanticTokenType RegularExpression { get; } = SemanticTokenType.Regexp;
    public static SemanticTokenType Operator { get; } = SemanticTokenType.Operator;
    public static SemanticTokenType Annotation { get; } = SemanticTokenType.Decorator;

    // LSP token types are extensible

    public static SemanticTokenType PropertyGet { get; } = new("propertyGet");
    public static SemanticTokenType PropertyLet { get; } = new("propertyLet");
    public static SemanticTokenType PropertySet { get; } = new("propertySet");
    public static SemanticTokenType Attribute { get; } = new("attribute");
    public static SemanticTokenType ClassHeader { get; } = new("header");
    public static SemanticTokenType Constant { get; } = new("const");
    public static SemanticTokenType BooleanLiteral { get; } = new("boolean");
    public static SemanticTokenType NullLiteral { get; } = new("null");
    public static SemanticTokenType NothingLiteral { get; } = new("nothing");
    public static SemanticTokenType EmptyLiteral { get; } = new("empty");
    public static SemanticTokenType DateLiteral { get; } = new("date");
    public static SemanticTokenType TypeHint { get; } = new("typeHint");
}

public readonly struct RubberduckSemanticTokenModifier
{
    public static SemanticTokenModifier[] SemanticTokenModifiers { get; } = typeof(RubberduckSemanticTokenModifier)
        .GetProperties(BindingFlags.Public | BindingFlags.Static)
        .Select(property => property.GetGetMethod())
        .Where(method => method != null && method.ReturnType == typeof(SemanticTokenModifier))
        .Select(method => (SemanticTokenModifier)method!.Invoke(null, null)!)
        .ToArray();

    public static SemanticTokenModifier Declaration { get; } = SemanticTokenModifier.Declaration;
    public static SemanticTokenModifier Definition { get; } = SemanticTokenModifier.Definition;
    public static SemanticTokenModifier ReadOnly { get; } = SemanticTokenModifier.Readonly;
    public static SemanticTokenModifier Static { get; } = SemanticTokenModifier.Static;
    public static SemanticTokenModifier Deprecated { get; } = SemanticTokenModifier.Deprecated;
    public static SemanticTokenModifier Abstract { get; } = SemanticTokenModifier.Abstract;
    public static SemanticTokenModifier Documentation { get; } = SemanticTokenModifier.Documentation;
    public static SemanticTokenModifier DefaultLibrary { get; } = SemanticTokenModifier.DefaultLibrary;

    // LSP token modifiers are extensible

    public static SemanticTokenModifier Optional { get; } = new("optional");
    public static SemanticTokenModifier LateBound { get; } = new("lateBound");
    public static SemanticTokenModifier Unreachable { get; } = new("unreachable");
}
