using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Model.Declarations.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations;

public abstract record class Symbol : DocumentSymbol
{
    protected Symbol(SymbolKind kind, string name, Uri uri, Accessibility accessibility, IEnumerable<Symbol>? children = default, IEnumerable<IParseTreeAnnotation>? annotations = default)
    {
        Kind = kind;
        Name = name;
        Uri = uri;
        Accessibility = accessibility;
        Children = new(children ?? []);
        Annotations = annotations?.ToArray() ?? [];
    }

    public bool IsUserDefined { get; init; }
    public Uri Uri { get; }

    public Accessibility Accessibility { get; }

    public IParseTreeAnnotation[] Annotations { get; } = [];
}

public record class ProjectSymbol : Symbol
{
    public ProjectSymbol(string name, Uri uri, IEnumerable<Symbol> children)
        : base(SymbolKind.Package, name, uri, Accessibility.Global, children)
    {
    }
}

public record class FileSymbol : Symbol
{
    public FileSymbol(string name, Uri uri, IEnumerable<Symbol> children)
        : base(SymbolKind.File, name, uri, Accessibility.Global, children)
    {
    }
}


public record class StandardModuleSymbol : Symbol
{
    public StandardModuleSymbol(string name, Uri uri, IEnumerable<Symbol> children)
        : base(SymbolKind.Module, name, uri, Accessibility.Global, children)
    {
    }
}

public record class ClassModuleSymbol : TypedSymbol<ClassModuleSymbol>
{
    public ClassModuleSymbol(Instancing instancing, string name, Uri uri, IEnumerable<Symbol> children, bool isUserDefined = false)
        : base(SymbolKind.Class, instancing == Instancing.Private ? Accessibility.Private : Accessibility.Public, name, uri, children, asTypeExpression: name)
    {
        ResolvedType = new VBClassType(name, uri, this, isUserDefined: isUserDefined);
        Instancing = instancing;
    }

    public Instancing Instancing { get; init; }
}

public abstract record class TypedSymbol<T> : Symbol, ITypedSymbol<T> where T : Symbol
{
    public TypedSymbol(SymbolKind kind, Accessibility accessibility, string name, Uri uri, IEnumerable<Symbol>? children, string? asTypeExpression)
        : base(kind, name, uri, accessibility, children)
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
    public ValuedTypedSymbol(SymbolKind kind, Accessibility accessibility, string name, Uri uri, string? asTypeExpression, string? valueExpression)
        : base(kind, accessibility, name, uri, [], asTypeExpression)
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
    public ParameterSymbol(string name, Uri uri, ParameterModifier modifier, string asTypeExpression)
        : base(SymbolKind.Variable, Accessibility.Private, name, uri, [], asTypeExpression)
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
    public ParamArrayParameterSymbol(string name, Uri uri, string asTypeExpression)
        : base(name, uri, ParameterModifier.ExplicitByRef, asTypeExpression)
    {
        IsArray = true;
    }
}

public record class OptionalParameterSymbol : ParameterSymbol
{
    public OptionalParameterSymbol(string name, Uri uri, ParameterModifier modifier, string asTypeExpression, string? valueExpression)
        : base(name, uri, modifier, asTypeExpression)
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
    public UserDefinedTypeSymbol(string name, Uri uri, Accessibility accessibility, IEnumerable<Symbol> children)
        : base(SymbolKind.Struct, accessibility, name, uri, children, name)
    {
        ResolvedType = new VBUserDefinedType(name, uri, this);
    }
}

public record class UserDefinedTypeMemberSymbol : TypedSymbol<UserDefinedTypeMemberSymbol>
{
    public UserDefinedTypeMemberSymbol(string name, Uri uri, string? typeName)
        : base(SymbolKind.Field, Accessibility.Public, name, uri, [], typeName)
    {
    }
}

public record class LibraryFunctionImportSymbol : TypedSymbol<LibraryFunctionImportSymbol>
{
    public LibraryFunctionImportSymbol(string library, string name, string? alias, Uri uri, Accessibility accessibility, IEnumerable<ParameterSymbol>? parameters, string? typeName)
        : base(SymbolKind.Function, accessibility, alias ?? name, uri, parameters, typeName)
    {
        Library = library;
        OriginalName = name;
    }

    public string Library { get; init; }
    public string? OriginalName { get; init; }
}

public record class LibraryProcedureImportSymbol : Symbol
{
    public LibraryProcedureImportSymbol(string library, string name, string? alias, Uri uri, Accessibility accessibility, IEnumerable<ParameterSymbol>? parameters)
        : base(SymbolKind.Method, alias ?? name, uri, accessibility, parameters)
    {
        Library = library;
        OriginalName = name;
    }

    public string Library { get; init; }
    public string? OriginalName { get; init; }
}

public record class FunctionSymbol : TypedSymbol<FunctionSymbol>
{
    public FunctionSymbol(string name, Uri uri, Accessibility accessibility, IEnumerable<ParameterSymbol>? parameters, IEnumerable<Symbol>? children, string? typeName)
        : base(SymbolKind.Function, accessibility, name, uri, parameters?.Concat(children ?? []).ToArray(), typeName)
    { }
}

public record class ProcedureSymbol : Symbol
{
    public ProcedureSymbol(string name, Uri uri, Accessibility accessibility, IEnumerable<ParameterSymbol>? parameters, IEnumerable<Symbol>? children)
        : base(SymbolKind.Method, name, uri, accessibility, parameters?.Concat(children ?? []).ToArray())
    { }
}

public record class PropertyGetSymbol : FunctionSymbol
{
    public PropertyGetSymbol(string name, Uri uri, Accessibility accessibility, IEnumerable<ParameterSymbol>? parameters, IEnumerable<Symbol>? children, string? asTypeNameExpression)
        : base(name, uri, accessibility, parameters, children, asTypeNameExpression)
    {
        Kind = SymbolKind.Property;
    }
}

public record class PropertyLetSymbol : ProcedureSymbol
{
    public PropertyLetSymbol(string name, Uri uri, Accessibility accessibility, IEnumerable<ParameterSymbol>? parameters, IEnumerable<Symbol>? children)
        : base(name, uri, accessibility, parameters, children)
    {
        Kind = SymbolKind.Property;
    }
}

public record class PropertySetSymbol : ProcedureSymbol
{
    public PropertySetSymbol(string name, Uri uri, Accessibility accessibility, IEnumerable<ParameterSymbol>? parameters, IEnumerable<Symbol>? children)
        : base(name, uri, accessibility, parameters, children)
    {
        Kind = SymbolKind.Property;
    }
}

public record class EnumSymbol : TypedSymbol<EnumSymbol>
{
    public EnumSymbol(string name, Uri uri, Accessibility accessibility, IEnumerable<EnumMemberSymbol> children, bool isUserDefined = false)
        : base(SymbolKind.Enum, accessibility, name, uri, children, name)
    {
        ResolvedType = new VBEnumType(name, uri, this, isUserDefined: isUserDefined);
    }
}

public record class EnumMemberSymbol : ValuedTypedSymbol
{
    public EnumMemberSymbol(string name, Uri uri, string typeName, string? value)
        : base(SymbolKind.EnumMember, Accessibility.Public, name, uri, typeName, value)
    {
    }
}

public record class EventMemberSymbol : ProcedureSymbol
{
    public EventMemberSymbol(string name, Uri uri, Accessibility accessibility, IEnumerable<ParameterSymbol>? parameters)
        : base(name, uri, accessibility, parameters, null)
    {
        Kind = SymbolKind.Event;
    }
}

public record class ConstantDeclarationSymbol : ValuedTypedSymbol
{
    public ConstantDeclarationSymbol(string name, Uri uri, Accessibility accessibility, string? asTypeNameExpression, string? valueExpression)
        : base(SymbolKind.Constant, accessibility, name, uri, asTypeNameExpression, valueExpression)
    { 
    }
}

public record class VariableDeclarationSymbol : TypedSymbol<VariableDeclarationSymbol>
{
    public VariableDeclarationSymbol(string name, Uri uri, Accessibility accessibility, string? asTypeNameExpression)
        : base(SymbolKind.Variable, accessibility, name, uri, [], asTypeNameExpression)
    {
    }
}

