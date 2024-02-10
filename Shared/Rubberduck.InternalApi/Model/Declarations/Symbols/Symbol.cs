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

public interface IValuedExpression<TValue> where TValue : VBTypedValue
{
    /// <summary>
    /// Evaluates the expression in the given context.
    /// </summary>
    /// <returns>Returns a <c>VBTypedValue</c> representing the result of the expression.</returns>
    TValue? Evaluate(ref VBExecutionScope context);
}

public interface IExecutable : IValuedExpression<VBTypedValue>
{
    /// <summary>
    /// Executes the symbol in the given context.
    /// </summary>
    /// <returns>
    /// Returns a <c>VBTypedValue</c> representing the result of the expression; <c>null</c> if the symbol is a non-returning executable member.
    /// </returns>
    VBTypedValue? Execute(ref VBExecutionContext context);
}

public abstract record class BooleanValuedExpression : IValuedExpression<VBBooleanValue>
{
    public abstract VBBooleanValue? Evaluate(ref VBExecutionScope context);
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
    public TypedSymbol WithResolvedType(VBType? resolvedType) => this with { ResolvedType = resolvedType };
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
                ? VBVariantType.TypeInfo // type not specified is an implicit variant
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

    public abstract VBTypedValue Evaluate(ref VBExecutionScope context);

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

    public VBTypedValue? Evaluate(ref VBExecutionScope context) => context.GetTypedValue(this);

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

public record class FunctionSymbol : TypedSymbol, IExecutable
{
    public FunctionSymbol(string name, Uri parentUri, Accessibility accessibility, IEnumerable<Symbol>? children = null, string? typeName = null, RubberduckSymbolKind kind = RubberduckSymbolKind.Function)
        : base(kind, accessibility, name, parentUri, (children ?? []).ToArray()) { }

    public bool? IsReachable { get; init; }

    public VBTypedValue? Evaluate(ref VBExecutionScope context)
    {
        // TODO walk the executable symbol tree to track the assigned value
        // for now we're happy just getting a resolved type back
        return context.GetTypedValue(this);
    }

    public VBTypedValue? Execute(ref VBExecutionContext context)
    {
        var member = context.GetModuleMember(this);
        if (member != null)
        {
            var scope = context.EnterScope(member);
            return Evaluate(ref scope);
        }
        throw VBRuntimeErrorException.PropertyOrMethodNotFound(this); // fitting, but is it really a VB-trappable error? or it's a .net-side bug?
    }
}

public record class ProcedureSymbol : TypedSymbol, IExecutable
{
    public ProcedureSymbol(string name, Uri parentUri, Accessibility accessibility, IEnumerable<Symbol>? children = null, RubberduckSymbolKind kind = RubberduckSymbolKind.Procedure)
        : base(kind, accessibility, name, parentUri, (children ?? []).ToArray(), VBLongPtrType.TypeInfo) { }

    public VBTypedValue? Evaluate(ref VBExecutionScope context) =>
        context.GetTypedValue(this) as VBLongPtrValue; // symbol table contains a VBLongPtrValue for procedures that can be used with the AddressOf operator.

    public VBTypedValue? Execute(ref VBExecutionContext context)
    {
        var member = context.GetModuleMember(this);
        if (member != null)
        {
            try
            {
                var scope = context.EnterScope(member);
                scope.Execute(ref context);
            }
            catch (VBCompileErrorException vbCompileError)
            {
                context.Diagnostics.Add(vbCompileError.Diagnostic);
                context.End();
            }
            catch (VBRuntimeErrorException vbRuntimeError)
            {
                // unhandled runtime error
                context.Diagnostics.Add(vbRuntimeError.Diagnostic);
            }
            return null;
        }
        throw VBRuntimeErrorException.PropertyOrMethodNotFound(this); // fitting, but is it really a VB-trappable error? or it's a .net-side bug?
    }
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

    public override VBTypedValue Evaluate(ref VBExecutionScope context) => context.GetTypedValue(this);
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

    public override VBTypedValue Evaluate(ref VBExecutionScope context) => context.GetTypedValue(this);
}

public record class VariableDeclarationSymbol : DeclarationExpressionSymbol
{
    public VariableDeclarationSymbol(string name, Uri parentUri, Accessibility accessibility, string? asTypeNameExpression)
        : base(RubberduckSymbolKind.Variable, name, parentUri, accessibility, children: [], annotations: [], asTypeNameExpression) { }
}

public record class StringLiteralSymbol : TypedSymbol
{
    public StringLiteralSymbol(string name, Uri parentUri)
        : base(RubberduckSymbolKind.StringLiteral, Accessibility.Undefined, name, parentUri, null, VBStringType.TypeInfo)
    {
    }
}

public record class NumberLiteralSymbol : TypedSymbol
{
    public NumberLiteralSymbol(string name, Uri parentUri)
        : base(RubberduckSymbolKind.NumberLiteral, Accessibility.Undefined, name, parentUri, null)
    {
    }
}

public abstract record class OperatorSymbol : TypedSymbol, IValuedExpression<VBTypedValue>
{
    public OperatorSymbol(string token, Uri parentUri, IEnumerable<Symbol>? children)
        : base(RubberduckSymbolKind.Operator, Accessibility.Undefined, token, parentUri, children, null)
    {
    }

    public VBTypedValue? Evaluate(ref VBExecutionScope context)
    {
        try
        {
            return EvaluateResult(ref context);
        }
        catch (VBCompileErrorException vbCompileError)
        {
            context = context.WithDiagnostics(vbCompileError.Diagnostics);
            return null;
        }
        catch (VBRuntimeErrorException vbRuntimeError)
        {
            context = context.WithError(vbRuntimeError);
            return null;
        }
    }

    protected abstract VBTypedValue? EvaluateResult(ref VBExecutionScope context);
}