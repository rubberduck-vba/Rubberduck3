using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations;

public abstract record class Symbol : DocumentSymbol
{
    protected Symbol(RubberduckSymbolKind kind, string name, Uri parentUri, Accessibility accessibility, IEnumerable<Symbol>? children = default, IEnumerable<IParseTreeAnnotation>? annotations = default)
    {
        Kind = (SymbolKind)kind;
        Name = name;
        ParentUri = parentUri;
        Uri = new(parentUri.OriginalString + $"#{name}");
        Accessibility = accessibility;
        Children = new(children ?? []);
        Annotations = annotations?.ToArray() ?? [];
    }

    public bool IsUserDefined { get; init; }
    public Uri ParentUri { get; }
    public Uri Uri { get; }

    public Accessibility Accessibility { get; }

    public IParseTreeAnnotation[] Annotations { get; } = [];
}

public record class ProjectSymbol : Symbol
{
    public ProjectSymbol(string name, Uri workspaceUri, IEnumerable<Symbol> children)
        : base(RubberduckSymbolKind.Project, name, workspaceUri, Accessibility.Global, children)
    {
    }
}

public record class FileSymbol : Symbol
{
    public FileSymbol(string name, Uri projectUri, IEnumerable<Symbol> children)
        : base(RubberduckSymbolKind.File, name, projectUri, Accessibility.Global, children)
    {
    }
}


public record class StandardModuleSymbol : Symbol
{
    public StandardModuleSymbol(string name, Uri fileUri, IEnumerable<Symbol> children)
        : base(RubberduckSymbolKind.Module, name, fileUri, Accessibility.Global, children)
    {
    }
}

public record class ClassModuleSymbol : TypedSymbol<ClassModuleSymbol>
{
    public ClassModuleSymbol(Instancing instancing, string name, Uri fileUri, IEnumerable<Symbol> children, bool isUserDefined = false)
        : base(RubberduckSymbolKind.Class, instancing == Instancing.Private ? Accessibility.Private : Accessibility.Public, name, fileUri, children, asTypeExpression: name)
    {
        ResolvedType = new VBClassType(name, fileUri, this, isUserDefined: isUserDefined);
        Instancing = instancing;
    }

    public Instancing Instancing { get; init; }
}

public abstract record class TypedSymbol<T> : Symbol, ITypedSymbol<T> where T : Symbol
{
    public TypedSymbol(RubberduckSymbolKind kind, Accessibility accessibility, string name, Uri parentUri, IEnumerable<Symbol>? children, string? asTypeExpression)
        : base(kind, name, parentUri, accessibility, children)
    {
        IsImplicit = string.IsNullOrWhiteSpace(asTypeExpression);
        AsTypeExpression = asTypeExpression ?? string.Empty;

        IsLateBound = IsImplicit
            || "variant".Equals(asTypeExpression, StringComparison.InvariantCultureIgnoreCase)
            || "object".Equals(asTypeExpression, StringComparison.InvariantCultureIgnoreCase);

        if (VBType.TryResolveIntrinsic(AsTypeExpression, out var type))
        {
            ResolvedType = type;
        }
    }

    public bool IsImplicit { get; init; }
    public bool IsLateBound { get; init; }
    public bool IsArray { get; init; }
    public string AsTypeExpression { get; init; }
    public VBType? ResolvedType { get; init; }

    public T ResolveType(VBType? resolvedType) => (this with { ResolvedType = resolvedType } as T)!;
}

/// <summary>
/// Represents a symbol that declares a value, e.g. a <c>Const</c>, or an <c>Enum</c> member.
/// </summary>
public abstract record class ValuedTypedSymbol : TypedSymbol<ValuedTypedSymbol>, IValuedSymbol<ValuedTypedSymbol>
{
    public ValuedTypedSymbol(RubberduckSymbolKind kind, Accessibility accessibility, string name, Uri parentUri, string? asTypeExpression, string? valueExpression)
        : base(kind, accessibility, name, parentUri, [], asTypeExpression)
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

    /// <summary>
    /// The symbol's resolved value.
    /// </summary>
    public object? ResolvedValue { get; init; }

    /// <summary>
    /// Gets a copy of this symbol with the specified resolved value.
    /// </summary>
    public ValuedTypedSymbol ResolveValue(object? resolvedValue) => this with { ResolvedValue = resolvedValue };
    /// <summary>
    /// Gets a copy of this symbol with the specified resolved value expression type.
    /// </summary>
    public ValuedTypedSymbol ResolveValueExpressionType(VBType? resolvedValueExpressionType) => this with { ResolvedValueExpressionType = resolvedValueExpressionType };
}

public record class ParameterSymbol : TypedSymbol<ParameterSymbol>
{
    public ParameterSymbol(string name, Uri parentUri, ParameterModifier modifier, string asTypeExpression)
        : base(RubberduckSymbolKind.Variable, Accessibility.Private, name, parentUri, [], asTypeExpression)
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
    public ParamArrayParameterSymbol(string name, Uri parentUri, string asTypeExpression)
        : base(name, parentUri, ParameterModifier.ExplicitByRef, asTypeExpression)
    {
        IsArray = true;
    }
}

public record class OptionalParameterSymbol : ParameterSymbol
{
    public OptionalParameterSymbol(string name, Uri parentUri, ParameterModifier modifier, string asTypeExpression, string? valueExpression)
        : base(name, parentUri, modifier, asTypeExpression)
    {
        ValueExpression = valueExpression;
    }

    public string? ValueExpression { get; init; }
    public VBType? ResolvedValueExpressionType { get; init; }

    /// <summary>
    /// Gets a copy of this symbol with the specified resolved value expression type.
    /// </summary>
    public OptionalParameterSymbol ResolveValueExpressionType(VBType? resolvedValueExpressionType) => this with { ResolvedValueExpressionType = resolvedValueExpressionType };
}

public record class UserDefinedTypeSymbol : TypedSymbol<UserDefinedTypeSymbol>
{
    public UserDefinedTypeSymbol(string name, Uri parentUri, Accessibility accessibility, IEnumerable<Symbol> children)
        : base(RubberduckSymbolKind.UserDefinedType, accessibility, name, parentUri, children, name)
    {
        ResolvedType = new VBUserDefinedType(name, parentUri, this);
    }
}

public record class UserDefinedTypeMemberSymbol : TypedSymbol<UserDefinedTypeMemberSymbol>
{
    public UserDefinedTypeMemberSymbol(string name, Uri parentUri, string? typeName)
        : base(RubberduckSymbolKind.Field, Accessibility.Public, name, parentUri, [], typeName) { }
}

public record class LibraryFunctionImportSymbol : TypedSymbol<LibraryFunctionImportSymbol>
{
    public LibraryFunctionImportSymbol(string library, string name, string? alias, Uri parentUri, Accessibility accessibility, IEnumerable<ParameterSymbol>? parameters, string? typeName)
        : base(RubberduckSymbolKind.Function, accessibility, alias ?? name, parentUri, parameters, typeName)
    {
        Library = library;
        OriginalName = name;
    }

    public string Library { get; init; }
    public string? OriginalName { get; init; }
}

public record class LibraryProcedureImportSymbol : Symbol
{
    public LibraryProcedureImportSymbol(string library, string name, string? alias, Uri parentUri, Accessibility accessibility, IEnumerable<ParameterSymbol>? parameters)
        : base(RubberduckSymbolKind.Procedure, alias ?? name, parentUri, accessibility, parameters)
    {
        Library = library;
        OriginalName = name;
    }

    public string Library { get; init; }
    public string? OriginalName { get; init; }
}

public record class FunctionSymbol : TypedSymbol<FunctionSymbol>
{
    public FunctionSymbol(string name, Uri parentUri, Accessibility accessibility, IEnumerable<ParameterSymbol>? parameters, IEnumerable<Symbol>? children, string? typeName, RubberduckSymbolKind kind = RubberduckSymbolKind.Function)
        : base(kind, accessibility, name, parentUri, parameters?.Concat(children ?? []).ToArray(), typeName) { }
}

public record class ProcedureSymbol : Symbol
{
    public ProcedureSymbol(string name, Uri parentUri, Accessibility accessibility, IEnumerable<ParameterSymbol>? parameters, IEnumerable<Symbol>? children, RubberduckSymbolKind kind = RubberduckSymbolKind.Procedure)
        : base(kind, name, parentUri, accessibility, parameters?.Concat(children ?? []).ToArray()) { }
}

public record class PropertyGetSymbol : FunctionSymbol
{
    public PropertyGetSymbol(string name, Uri parentUri, Accessibility accessibility, IEnumerable<ParameterSymbol>? parameters, IEnumerable<Symbol>? children, string? asTypeNameExpression)
        : base(name, parentUri, accessibility, parameters, children, asTypeNameExpression, RubberduckSymbolKind.Property) { }
}

public record class PropertyLetSymbol : ProcedureSymbol
{
    public PropertyLetSymbol(string name, Uri parentUri, Accessibility accessibility, IEnumerable<ParameterSymbol>? parameters, IEnumerable<Symbol>? children)
        : base(name, parentUri, accessibility, parameters, children, RubberduckSymbolKind.Property) { }
}

public record class PropertySetSymbol : ProcedureSymbol
{
    public PropertySetSymbol(string name, Uri parentUri, Accessibility accessibility, IEnumerable<ParameterSymbol>? parameters, IEnumerable<Symbol>? children)
        : base(name, parentUri, accessibility, parameters, children, RubberduckSymbolKind.Property) { }
}

public record class EnumSymbol : TypedSymbol<EnumSymbol>
{
    public EnumSymbol(string name, Uri parentUri, Accessibility accessibility, IEnumerable<EnumMemberSymbol> children, bool isUserDefined = false)
        : base(RubberduckSymbolKind.Enum, accessibility, name, parentUri, children, name) 
    {
        ResolvedType = new VBEnumType(name, parentUri, this, isUserDefined: isUserDefined);
    }
}

public record class EnumMemberSymbol : ValuedTypedSymbol
{
    public EnumMemberSymbol(string name, Uri parentUri, string typeName, string? value)
        : base(RubberduckSymbolKind.EnumMember, Accessibility.Public, name, parentUri, typeName, value) { }
}

public record class EventMemberSymbol : ProcedureSymbol
{
    public EventMemberSymbol(string name, Uri parentUri, Accessibility accessibility, IEnumerable<ParameterSymbol>? parameters)
        : base(name, parentUri, accessibility, parameters, null, RubberduckSymbolKind.Event) { }
}

public record class ConstantDeclarationSymbol : ValuedTypedSymbol
{
    public ConstantDeclarationSymbol(string name, Uri parentUri, Accessibility accessibility, string? asTypeNameExpression, string? valueExpression)
        : base(RubberduckSymbolKind.Constant, accessibility, name, parentUri, asTypeNameExpression, valueExpression) { }
}

public record class VariableDeclarationSymbol : TypedSymbol<VariableDeclarationSymbol>
{
    public VariableDeclarationSymbol(string name, Uri parentUri, Accessibility accessibility, string? asTypeNameExpression)
        : base(RubberduckSymbolKind.Variable, accessibility, name, parentUri, [], asTypeNameExpression) { }
}

