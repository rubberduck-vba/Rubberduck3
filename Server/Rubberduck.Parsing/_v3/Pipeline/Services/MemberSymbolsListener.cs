﻿using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.Parsing.Grammar;

namespace Rubberduck.Parsing._v3.Pipeline.Services;

public interface IVBListener<TResult> : IVBAParserListener
{
    TResult Result { get; }
}

public class MemberSymbolsListener : VBAParserBaseListener, IVBListener<Symbol>
{
    private readonly WorkspaceFileUri _workspaceFileUri;

    private bool _hasModuleHeader;
    private string _vbNameAttributeValue = null!;

    /// <summary>
    /// Accumulates symbols to be added to the current context.
    /// </summary>
    /// <remarks>
    /// Push an empty list when entering a new context; Pop the child symbols list when creating the current symbol when exiting the context.
    /// </remarks>
    private readonly Stack<List<(Type TContext, Symbol Symbol)>> _currentSymbolChildren = [];
    private Type? _currentContextType;

    public MemberSymbolsListener(WorkspaceFileUri uri)
    {
        _workspaceFileUri = uri;
    }

    public Symbol Result { get; private set; } = null!;

    private void OnChildSymbol<TContext>(Symbol symbol, TContext? _ = null) where TContext :ParserRuleContext
        => _currentSymbolChildren.Peek().Add((typeof(TContext), symbol));


    private void OnEnterNewCurrentSymbol<TContext>(TContext? _ = null) where TContext : ParserRuleContext
        => _currentSymbolChildren.Push([]);

    private Symbol CreateCurrentSymbol<TContext>(Func<IEnumerable<Symbol>, Symbol> create, TContext? _ = null) where TContext : ParserRuleContext
    {
        if (typeof(TContext) != _currentContextType)
        {
            throw new InvalidOperationException($"BUG: Expected current symbol under current context type {_currentContextType?.Name ?? "(null)"}, but context is {typeof(TContext).Name}.");
        }

        var children = _currentSymbolChildren.Pop().Select(node => node.Symbol).ToArray();
        return create(children);
    }

    public override void EnterModule([NotNull] VBAParser.ModuleContext context)
        => OnEnterNewCurrentSymbol(context);

    public override void ExitModule([NotNull] VBAParser.ModuleContext context)
    {
        Symbol moduleSymbol;
        if (_hasModuleHeader)
        {
            var instancing = Instancing.Private; // TODO get from attributes
            moduleSymbol = CreateCurrentSymbol(children => new ClassModuleSymbol(instancing, _vbNameAttributeValue, _workspaceFileUri, children), context);
        }
        else
        {
            moduleSymbol = CreateCurrentSymbol(children => new StandardModuleSymbol(_vbNameAttributeValue, _workspaceFileUri, children), context);
        }

        Result = moduleSymbol;
    }

    public override void ExitModuleHeader([NotNull] VBAParser.ModuleHeaderContext context)
        => _hasModuleHeader = context.ChildCount > 0;

    protected InternalApi.Model.Accessibility GetAccessibility(VBAParser.VisibilityContext context)
    {
        if (context.PRIVATE() != null)
        {
            return InternalApi.Model.Accessibility.Private;
        }

        if (context.PUBLIC() != null)
        {
            return InternalApi.Model.Accessibility.Public;
        }

        if (context.FRIEND() != null)
        {
            return InternalApi.Model.Accessibility.Friend;
        }

        if (context.GLOBAL() != null)
        {
            return InternalApi.Model.Accessibility.Global;
        }

        return InternalApi.Model.Accessibility.Implicit;
    }

    protected ParameterModifier GetModifier(VBAParser.ArgContext context)
    {
        if (context.BYREF() != null)
        {
            return ParameterModifier.ExplicitByRef;
        }
        else if (context.BYVAL() != null)
        {
            return ParameterModifier.ExplicitByVal;
        }
        else if (_currentContextType == typeof(VBAParser.PropertyLetStmtContext)
              || _currentContextType == typeof(VBAParser.PropertySetStmtContext))
        {
            return ParameterModifier.ImplicitByVal;
        }

        return ParameterModifier.ImplicitByRef;
    }

    protected string GetIdentifierNameTokenText(VBAParser.UnrestrictedIdentifierContext context)
        => GetIdentifierNameTokenText(context.identifier());
    protected string GetIdentifierNameTokenText(VBAParser.IdentifierContext context)
    {
        if (context.typedIdentifier() is null)
        {
            return GetIdentifierNameTokenText(context.untypedIdentifier());
        }

        return GetIdentifierNameTokenText(context.typedIdentifier());
    }
    protected string GetIdentifierNameTokenText(VBAParser.TypedIdentifierContext context)
        => GetIdentifierNameTokenText(context.untypedIdentifier());
    protected string GetIdentifierNameTokenText(VBAParser.UntypedIdentifierContext context)
        => GetIdentifierNameTokenText(context.identifierValue());
    protected string GetIdentifierNameTokenText(VBAParser.IdentifierValueContext context)
        => context.IDENTIFIER().GetText();

    protected string? GetAsTypeExpressionText(VBAParser.AsTypeClauseContext? context)
        => context?.type().GetText();

    public override void ExitArg([NotNull] VBAParser.ArgContext context)
    {
        string name = GetIdentifierNameTokenText(context.unrestrictedIdentifier());
        var asTypeExpression = GetAsTypeExpressionText(context.asTypeClause());
        var modifier = GetModifier(context);

        var parentUri = _workspaceFileUri.GetChildSymbolUri(name);

        Symbol symbol;
        if (context.OPTIONAL() != null)
        {
            var valueExpression = context.argDefaultValue().expression().GetText();
            symbol = new OptionalParameterSymbol(name, parentUri, modifier, asTypeExpression, valueExpression);
        }
        else if (context.PARAMARRAY() != null)
        {
            symbol = new ParamArrayParameterSymbol(name, parentUri, asTypeExpression);
        }
        else
        {
            symbol = new ParameterSymbol(name, parentUri, modifier, asTypeExpression);
        }

        OnChildSymbol(symbol, context);
    }

    public override void EnterDeclareStmt([NotNull] VBAParser.DeclareStmtContext context)
        => OnEnterNewCurrentSymbol(context);

    public override void ExitDeclareStmt([NotNull] VBAParser.DeclareStmtContext context)
    {
        var isPtrSafe = context.PTRSAFE() != null;
        var name = context.identifier().GetText();
        var accessibility = GetAccessibility(context.visibility());
        var asTypeExpression = GetAsTypeExpressionText(context.asTypeClause());

        var library = context.STRINGLITERAL()[0].GetText().UnQuote();

        string? alias = null;
        if (context.ALIAS() != null)
        {
            alias = context.STRINGLITERAL()[1].GetText().UnQuote();
        }

        Symbol symbol;
        if (context.FUNCTION() != null)
        {
            symbol = CreateCurrentSymbol(children => new LibraryFunctionImportSymbol(library, name, alias, isPtrSafe, _workspaceFileUri, accessibility, children.OfType<ParameterSymbol>(), asTypeExpression), context);
        }
        else //if (context.SUB() != null)
        {
            symbol = CreateCurrentSymbol(children => new LibraryProcedureImportSymbol(library, name, alias, isPtrSafe, _workspaceFileUri, accessibility, children.OfType<ParameterSymbol>()), context);
        }

        OnChildSymbol(symbol, context);
    }

    public override void EnterEventStmt([NotNull] VBAParser.EventStmtContext context)
        => OnEnterNewCurrentSymbol(context);
    public override void ExitEventStmt([NotNull] VBAParser.EventStmtContext context)
    {
        var name = context.identifier().GetText();
        var accessibility = GetAccessibility(context.visibility());

        OnChildSymbol(CreateCurrentSymbol(children => new EventMemberSymbol(name, _workspaceFileUri, accessibility, children.OfType<ParameterSymbol>()), context), context);
    }

    public override void EnterEnumerationStmt([NotNull] VBAParser.EnumerationStmtContext context)
        => OnEnterNewCurrentSymbol(context);

    public override void ExitEnumerationStmt([NotNull] VBAParser.EnumerationStmtContext context)
    {
        var name = context.identifier().GetText();
        var accessibility = GetAccessibility(context.visibility());

        OnChildSymbol(CreateCurrentSymbol(children => new EnumSymbol(name, _workspaceFileUri, accessibility, children.OfType<EnumMemberSymbol>()), context), context);
    }
    public override void ExitEnumerationStmt_Constant([NotNull] VBAParser.EnumerationStmt_ConstantContext context)
    {
        var name = context.identifier().GetText();
        var valueExpression = context.expression().GetText();

        OnChildSymbol(new EnumMemberSymbol(name, _workspaceFileUri, valueExpression), context);
    }

    public override void EnterUdtDeclaration([NotNull] VBAParser.UdtDeclarationContext context)
        => OnEnterNewCurrentSymbol(context);

    public override void ExitUdtDeclaration([NotNull] VBAParser.UdtDeclarationContext context)
    {
        var name = GetIdentifierNameTokenText(context.untypedIdentifier());
        var accessibility = GetAccessibility(context.visibility());

        OnChildSymbol(CreateCurrentSymbol(children => new UserDefinedTypeSymbol(name, _workspaceFileUri, accessibility, children.OfType<UserDefinedTypeMemberSymbol>()), context), context);
    }
    public override void ExitUdtMember([NotNull] VBAParser.UdtMemberContext context)
    {
        string name;
        string? asTypeExpression;

        var nameContext = context.reservedNameMemberDeclaration();
        if (nameContext != null)
        {
            name = GetIdentifierNameTokenText(nameContext.unrestrictedIdentifier());
            asTypeExpression = GetAsTypeExpressionText(nameContext.asTypeClause());
        }
        else
        {
            var declaration = context.untypedNameMemberDeclaration();
            name = GetIdentifierNameTokenText(declaration.untypedIdentifier());
            asTypeExpression = GetAsTypeExpressionText(declaration.optionalArrayClause()?.asTypeClause());
        }

        OnChildSymbol(CreateCurrentSymbol(children => new UserDefinedTypeMemberSymbol(name, _workspaceFileUri, asTypeExpression), context), context);
    }

    public override void EnterSubStmt([NotNull] VBAParser.SubStmtContext context)
        => OnEnterNewCurrentSymbol(context);

    public override void ExitSubStmt([NotNull] VBAParser.SubStmtContext context)
    {
        var name = GetIdentifierNameTokenText(context.subroutineName().identifier());
        var accessibility = GetAccessibility(context.visibility());
        
        OnChildSymbol(CreateCurrentSymbol(children => new ProcedureSymbol(name, _workspaceFileUri, accessibility, children), context), context);
    }

    public override void EnterFunctionStmt([NotNull] VBAParser.FunctionStmtContext context)
        => OnEnterNewCurrentSymbol(context);

    public override void ExitFunctionStmt([NotNull] VBAParser.FunctionStmtContext context)
    {
        var name = GetIdentifierNameTokenText(context.functionName().identifier());
        var accessibility = GetAccessibility(context.visibility());
        var typeName = context.asTypeClause()?.GetText();

        OnChildSymbol(CreateCurrentSymbol(children => new FunctionSymbol(name, _workspaceFileUri, accessibility, children, typeName), context), context);
    }

    public override void EnterPropertyGetStmt([NotNull] VBAParser.PropertyGetStmtContext context)
        => OnEnterNewCurrentSymbol(context);

    public override void ExitPropertyGetStmt([NotNull] VBAParser.PropertyGetStmtContext context)
    {
        var name = GetIdentifierNameTokenText(context.functionName().identifier());
        var accessibility = GetAccessibility(context.visibility());
        var typeName = context.asTypeClause()?.GetText();

        OnChildSymbol(CreateCurrentSymbol(children => new PropertyGetSymbol(name, _workspaceFileUri, accessibility, children, typeName), context), context);
    }

    public override void EnterPropertyLetStmt([NotNull] VBAParser.PropertyLetStmtContext context)
        => OnEnterNewCurrentSymbol(context);

    public override void ExitPropertyLetStmt([NotNull] VBAParser.PropertyLetStmtContext context)
    {
        var name = GetIdentifierNameTokenText(context.subroutineName().identifier());
        var accessibility = GetAccessibility(context.visibility());

        OnChildSymbol(CreateCurrentSymbol(children => new PropertyLetSymbol(name, _workspaceFileUri, accessibility, children), context), context);
    }

    public override void EnterPropertySetStmt([NotNull] VBAParser.PropertySetStmtContext context)
        => OnEnterNewCurrentSymbol(context);

    public override void ExitPropertySetStmt([NotNull] VBAParser.PropertySetStmtContext context)
    {
        var name = GetIdentifierNameTokenText(context.subroutineName().identifier());
        var accessibility = GetAccessibility(context.visibility());

        OnChildSymbol(CreateCurrentSymbol(children => new PropertySetSymbol(name, _workspaceFileUri, accessibility, children), context), context);
    }
}
