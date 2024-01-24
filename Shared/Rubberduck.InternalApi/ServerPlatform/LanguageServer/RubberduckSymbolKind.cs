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

    File = SymbolKind.File,
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
    Keyword = SymbolKind.Key,
    Nothing = SymbolKind.Null,
    EnumMember = SymbolKind.EnumMember,
    UserDefinedType = SymbolKind.Struct,
    Event = SymbolKind.Event,
    Operator = SymbolKind.Operator,
    //TypeParameter = SymbolKind.TypeParameter, /* no generics in Classic-VB */

    /* RD3: start extension values at 1024 to avoid any possible clash with a future LSP version
     * Unlike semantic token types and their modifiers, it's not clearly stated anywhere that this type is intended to be extended.
     */
    Comment = 1024,
    Annotation,
    LineLabel,

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

    public static SemanticTokenType Attribute { get; } = new SemanticTokenType("attribute");
    public static SemanticTokenType ClassHeader { get; } = new SemanticTokenType("header");
    public static SemanticTokenType BooleanLiteral { get; } = new SemanticTokenType("boolean");
    public static SemanticTokenType NullLiteral { get; } = new SemanticTokenType("null");
    public static SemanticTokenType NothingLiteral { get; } = new SemanticTokenType("nothing");
    public static SemanticTokenType EmptyLiteral { get; } = new SemanticTokenType("empty");
    public static SemanticTokenType DateLiteral { get; } = new SemanticTokenType("date");
    public static SemanticTokenType HexLiteral { get; } = new SemanticTokenType("hex");
    public static SemanticTokenType OctLiteral { get; } = new SemanticTokenType("oct");
    public static SemanticTokenType TypeHint { get; } = new SemanticTokenType("typeHint");
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

    public static SemanticTokenModifier Optional { get; } = new SemanticTokenModifier("optional");
    public static SemanticTokenModifier LateBound { get; } = new SemanticTokenModifier("lateBound");
}
