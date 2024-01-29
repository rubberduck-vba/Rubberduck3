using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Symbols;

public interface IExecutableSymbol
{
    /// <summary>
    /// Indicates whether an executable symbol is reachable or not.
    /// </summary>
    /// <remarks>
    /// <c>null</c> if unknown, <c>false</c> only if determined to be unreachable.
    /// </remarks>
    bool? IsReachable { get; init; }

    /// <summary>
    /// Executes the symbol in the specified execution context.
    /// </summary>
    /// <remarks>
    /// Returns the altered execution context.
    /// </remarks>
    ExecutionContext Execute(ExecutionContext context);
}

/// <summary>
/// Represents a document symbol, as defined by LSP.
/// </summary>
public abstract record class Symbol : DocumentSymbol
{
    protected Symbol(RubberduckSymbolKind kind, string name, Uri? parentUri = null, Accessibility accessibility = Accessibility.Undefined, IEnumerable<Symbol>? children = default)
    {
        Kind = (SymbolKind)kind;
        Name = name;
        ParentUri = parentUri ?? new Uri($"vb://symbols/{kind}");       
        Uri = ParentUri.GetChildSymbolUri(name);
        Children = new(children ?? []);
    }

    /// <summary>
    /// <c>true</c> if the symbol is user-defined, <c>false</c> if the symbol is defined in a referenced library.
    /// </summary>
    public bool IsUserDefined { get; init; }
    /// <summary>
    /// The URI of the parent symbol.
    /// </summary>
    /// <remarks>
    /// This information should not be used to construct a symbol hierarchy.
    /// </remarks>
    public Uri ParentUri { get; init; }
    /// <summary>
    /// The URI of the symbol as a fragment of the parent URI.
    /// </summary>
    /// <remarks>
    /// Formed with the original string of the parent URI concatenated with a '#' followed by the <c>Name</c> of the symbol.
    /// </remarks>
    public Uri Uri { get; init; }

    public Symbol WithName(string name) => this with { Name = name, Uri = ParentUri.GetChildSymbolUri(name) };
    public Symbol WithParentUri(Uri parentUri) => this with { Uri = parentUri.GetChildSymbolUri(Name), ParentUri = parentUri };
    public Symbol WithChildren(IEnumerable<Symbol> children) => this with { Children = new(children ?? []) };
}

/// <summary>
/// Represents a symbol that can be resolved to a <c>VBType</c>.
/// </summary>
public abstract record class TypedSymbol : Symbol, ITypedSymbol
{
    public TypedSymbol(RubberduckSymbolKind kind, Accessibility accessibility, string name, Uri? parentUri = null, IEnumerable<Symbol>? children = null, VBType? type = null)
        : base(kind, name, parentUri, accessibility, children)
    {
        Accessibility = accessibility;
        ResolvedType = type;
    }

    /// <summary>
    /// Determines whether a symbol is accessible beyond its enclosing scope.
    /// </summary>
    /// <remarks>
    /// Usually driven by an access modifier token, but not necessarily.
    /// </remarks>
    public Accessibility Accessibility { get; }

    public VBType? ResolvedType { get; init; }
    public ITypedSymbol WithResolvedType(VBType? resolvedType) => this with { ResolvedType = resolvedType };
}

public abstract record class DeclarationExpressionSymbol : TypedSymbol, IDeclaredTypeSymbol
{
    protected DeclarationExpressionSymbol(RubberduckSymbolKind kind, string name, Uri parentUri, Accessibility accessibility, IEnumerable<Symbol>? children = null, IEnumerable<IParseTreeAnnotation>? annotations = null, string? asTypeExpression = null, VBType? type = null)
        : base(kind, accessibility, name, parentUri, children, ResolveVariantIfUnspecified(type, asTypeExpression))
    {
    }

    private static VBType? ResolveVariantIfUnspecified(VBType? type, string? asTypeExpression)
    {
        if (type is null)
        {
            return string.IsNullOrWhiteSpace(asTypeExpression) 
                ? VBType.VbVariantType // type not specified is an implicit variant
                : null; // type must be resolved later
        }

        return type;
    }

    public string? AsTypeExpression { get; init; }
}

/// <summary>
/// Represents a symbol that declares a value, e.g. a <c>Const</c>, or an <c>Enum</c> member.
/// </summary>
public abstract record class ValuedTypedSymbol : TypedSymbol, IValuedSymbol
{
    public ValuedTypedSymbol(RubberduckSymbolKind kind, Accessibility accessibility, string name, Uri parentUri, string? asTypeExpression, string? valueExpression)
        : base(kind, accessibility, name, parentUri, [])
    {
        ValueExpression = valueExpression;
    }

    /// <summary>
    /// The symbol's declared value expression.
    /// </summary>
    public string? ValueExpression { get; init; }
    /// <summary>
    /// The resolved type of the symbol's value expression.
    /// </summary>
    /// <remarks>
    /// May or may not match the symbol's declared type, or safely convert to it.
    /// </remarks>
    public VBType? ResolvedValueExpressionType { get; init; }
    public ITypedSymbol WithResolvedValueExpressionType(VBType? type) => this with { ResolvedValueExpressionType = type };
}

public record class ParameterSymbol : DeclarationExpressionSymbol
{
    public ParameterSymbol(string name, Uri parentUri, ParameterModifier modifier, string? asTypeExpression)
        : base(RubberduckSymbolKind.Variable, name, parentUri, Accessibility.Private, asTypeExpression: asTypeExpression)
    {
        Modifier = new(modifier);
    }

    /// <summary>
    /// Gets information about this parameter's modifier expression.
    /// </summary>
    public ParameterSymbolModifier Modifier { get; }
}

public record class ParamArrayParameterSymbol : ParameterSymbol
{
    public ParamArrayParameterSymbol(string name, Uri parentUri, string? asTypeExpression)
        : base(name, parentUri, ParameterModifier.ExplicitByRef, asTypeExpression)
    {
    }
}

public record class OptionalParameterSymbol : ParameterSymbol, IValuedSymbol
{
    public OptionalParameterSymbol(string name, Uri parentUri, ParameterModifier modifier, string? asTypeExpression, string? valueExpression)
        : base(name, parentUri, modifier, asTypeExpression)
    {
        ValueExpression = valueExpression;
    }

    public string? ValueExpression { get; init; }
    public VBType? ResolvedValueExpressionType { get; init; }

    public ITypedSymbol WithResolvedValueExpressionType(VBType? resolvedValueExpressionType) => this with { ResolvedValueExpressionType = resolvedValueExpressionType };
}

public record class UserDefinedTypeSymbol : TypedSymbol
{
    public UserDefinedTypeSymbol(string name, Uri parentUri, Accessibility accessibility, IEnumerable<UserDefinedTypeMemberSymbol> children)
        : base(RubberduckSymbolKind.UserDefinedType, accessibility, name, parentUri, children.Cast<Symbol>())
    {
        ResolvedType = new VBUserDefinedType(name, parentUri, this);
    }
}

public record class UserDefinedTypeMemberSymbol : DeclarationExpressionSymbol
{
    public UserDefinedTypeMemberSymbol(string name, Uri parentUri, string? asTypeExpression)
        : base(RubberduckSymbolKind.Field, name, parentUri, Accessibility.Public, asTypeExpression: asTypeExpression) { }
}

public record class LibraryFunctionImportSymbol : FunctionSymbol
{
    public LibraryFunctionImportSymbol(string library, string name, string? alias, bool isPtrSafe, Uri parentUri, Accessibility accessibility, IEnumerable<ParameterSymbol>? parameters, string? typeName)
        : base(name, parentUri, accessibility, parameters, typeName)
    {
        Library = library;
        OriginalName = name;
        Alias = alias;
        IsPtrSafe = isPtrSafe;
    }

    public bool IsPtrSafe { get; init; }
    public string Library { get; init; }
    public string? OriginalName { get; init; }
    public string? Alias { get; init; }
}

public record class LibraryProcedureImportSymbol : ProcedureSymbol
{
    public LibraryProcedureImportSymbol(string library, string name, string? alias, bool isPtrSafe, Uri parentUri, Accessibility accessibility, IEnumerable<ParameterSymbol>? parameters)
        : base(name, parentUri, accessibility, parameters)
    {
        Library = library;
        OriginalName = name;
        Alias = alias;
        IsPtrSafe = isPtrSafe;
    }

    public bool IsPtrSafe { get; init; }
    public string Library { get; init; }
    public string? OriginalName { get; init; }
    public string? Alias { get; init; }
}

public record class FunctionSymbol : TypedSymbol, IExecutableSymbol
{
    public FunctionSymbol(string name, Uri parentUri, Accessibility accessibility, IEnumerable<Symbol>? children = null, string? typeName = null, RubberduckSymbolKind kind = RubberduckSymbolKind.Function)
        : base(kind, accessibility, name, parentUri, (children ?? []).ToArray()) { }

    public bool? IsReachable { get; init; }
    public VBTypedValue? Evaluate(ExecutionContext context) => null;
    public ExecutionContext Execute(ExecutionContext context) => context;
}

public record class ProcedureSymbol : Symbol, IExecutableSymbol
{
    public ProcedureSymbol(string name, Uri parentUri, Accessibility accessibility, IEnumerable<Symbol>? children = null, RubberduckSymbolKind kind = RubberduckSymbolKind.Procedure)
        : base(kind, name, parentUri, accessibility, (children ?? []).ToArray()) { }

    public bool? IsReachable { get; init; }
    public VBTypedValue? Evaluate(ExecutionContext context) => null;
    public ExecutionContext Execute(ExecutionContext context) => context;
}

public record class PropertyGetSymbol : FunctionSymbol
{
    public PropertyGetSymbol(string name, Uri parentUri, Accessibility accessibility, IEnumerable<Symbol>? children = null, string? asTypeNameExpression = null)
        : base(name, parentUri, accessibility, (children ?? []).ToArray(), asTypeNameExpression, RubberduckSymbolKind.Property) { }
}

public record class PropertyLetSymbol : ProcedureSymbol
{
    public PropertyLetSymbol(string name, Uri parentUri, Accessibility accessibility, IEnumerable<Symbol>? children = null)
        : base(name, parentUri, accessibility, (children ?? []).ToArray(), RubberduckSymbolKind.Property) { }
}

public record class PropertySetSymbol : ProcedureSymbol
{
    public PropertySetSymbol(string name, Uri parentUri, Accessibility accessibility, IEnumerable<Symbol>? children = null)
        : base(name, parentUri, accessibility, (children ?? []).ToArray(), RubberduckSymbolKind.Property) { }
}

public record class EnumSymbol : TypedSymbol
{
    public EnumSymbol(string name, Uri parentUri, Accessibility accessibility, IEnumerable<EnumMemberSymbol>? children = null, bool isUserDefined = false)
        : base(RubberduckSymbolKind.Enum, accessibility, name, parentUri, children)
    {
        ResolvedType = new VBEnumType(name, parentUri, this, isUserDefined: isUserDefined);
    }
}

public record class EnumMemberSymbol : ValuedTypedSymbol
{
    public EnumMemberSymbol(string name, Uri parentUri, string? value)
        : base(RubberduckSymbolKind.EnumMember, Accessibility.Public, name, parentUri, null, value) { }


}

public record class EventMemberSymbol : ProcedureSymbol
{
    public EventMemberSymbol(string name, Uri parentUri, Accessibility accessibility, IEnumerable<ParameterSymbol>? parameters)
        : base(name, parentUri, accessibility, parameters, RubberduckSymbolKind.Event) { }
}

public record class ConstantDeclarationSymbol : ValuedTypedSymbol
{
    public ConstantDeclarationSymbol(string name, Uri parentUri, Accessibility accessibility, string? asTypeNameExpression, string? valueExpression)
        : base(RubberduckSymbolKind.Constant, accessibility, name, parentUri, asTypeNameExpression, valueExpression) { }
}

public record class VariableDeclarationSymbol : DeclarationExpressionSymbol
{
    public VariableDeclarationSymbol(string name, Uri parentUri, Accessibility accessibility, string? asTypeNameExpression)
        : base(RubberduckSymbolKind.Variable, name, parentUri, accessibility, children: [], annotations: [], asTypeNameExpression) { }
}

public record class StringLiteralSymbol : TypedSymbol
{
    public StringLiteralSymbol(string name, Uri parentUri)
        : base(RubberduckSymbolKind.StringLiteral, Accessibility.Undefined, name, parentUri, null, VBType.VbStringType)
    {
    }
}

public record class OperatorSymbol : TypedSymbol
{
    public OperatorSymbol(string token, Uri parentUri, IEnumerable<Symbol>? children)
        : base(RubberduckSymbolKind.Operator, Accessibility.Undefined, token, parentUri, children, null)
    {
    }
}