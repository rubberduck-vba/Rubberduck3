using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.Parsing.Grammar;

namespace Rubberduck.Parsing.Listeners;

public class VBSymbolListener : VBAParserBaseListener
{
    private readonly string _fileName;
    private readonly WorkspaceUri _workspaceUri;
    private readonly WorkspaceFileUri _workspaceFileUri;

    private bool _hasModuleHeader;
    private string _vbNameAttributeValue;

    /// <summary>
    /// Accumulates symbols to be added to the current context.
    /// </summary>
    /// <remarks>
    /// Push an empty list when entering a new context; Pop the child symbols list when creating the current symbol when exiting the context.
    /// </remarks>
    private readonly Stack<List<(Type TContext,Symbol Symbol)>> _currentSymbolChildren = [];
    private Type? _currentContextType;

    public VBSymbolListener(string fileName, WorkspaceUri workspaceRoot)
    {
        _fileName = fileName;
        _workspaceUri = workspaceRoot;
        _workspaceFileUri = new(fileName, workspaceRoot);
    }

    public Symbol Result { get; private set; }
    private void OnEnterNewCurrentSymbol<TContext>(TContext? context = null) where TContext : ParserRuleContext
        => _currentSymbolChildren.Push([]);

    private Symbol CreateCurrentSymbol<TContext>(Func<IEnumerable<Symbol>, Symbol> create, TContext? _ = null) where TContext:  ParserRuleContext
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

    public override void EnterSubStmt([NotNull] VBAParser.SubStmtContext context)
        => OnEnterNewCurrentSymbol(context);

    public override void ExitArg([NotNull] VBAParser.ArgContext context)
    {
        string name = context.unrestrictedIdentifier().identifier().GetText();
        var asTypeExpression = context.asTypeClause()?.type().GetText();

        var parentUri = _workspaceFileUri.GetChildSymbolUri(name);

        ParameterModifier modifier;
        if (context.BYREF() != null)
        {
            modifier = ParameterModifier.ExplicitByRef;
        }
        else if (context.BYVAL() != null)
        {
            modifier = ParameterModifier.ExplicitByVal;
        }
        else if (_currentContextType == typeof(VBAParser.PropertyLetStmtContext) 
              || _currentContextType == typeof(VBAParser.PropertySetStmtContext))
        {
            modifier = ParameterModifier.ImplicitByVal;
        }
        else
        {
            modifier = ParameterModifier.ImplicitByRef;
        }

        Symbol symbol;
        if (context.OPTIONAL() != null)
        {
            var valueExpression = context.argDefaultValue()?.GetText();
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

        _currentSymbolChildren.Peek().Add((context.GetType(), symbol));
    }

    public override void ExitSubStmt([NotNull] VBAParser.SubStmtContext context)
    {
        
    }
}

public class VBFoldingListener : VBAParserBaseListener
{
    private readonly IBlockFoldingSettings _settings;
    private readonly List<FoldingRange> _foldings = [];
    public IEnumerable<FoldingRange> Foldings => _foldings.OrderBy(e => e.StartLine);

    public VBFoldingListener(IBlockFoldingSettings settings)
    {
        _settings = settings;
    }

    private FoldingRange Fold(string text, FoldingRangeKind kind, IToken start, IToken end)
    {
        return new()
        {
            CollapsedText = text,
            Kind = kind,
            // IToken.Line: 1..n
            StartLine = start.Line,
            EndLine = end.Line,
            // IToken.Column: 0..n
            StartCharacter = 1 + start.Column,
            EndCharacter = 1 + end.Column,
        };
    }

    public override void EnterModule([NotNull] VBAParser.ModuleContext context)
    {
        _foldings.Clear();
    }

    public override void ExitModuleConfig([NotNull] VBAParser.ModuleConfigContext context)
    {
        if ((_settings?.FoldModuleHeader ?? true) && context.ChildCount > 0 && context.Offset.Length > 0)
        {
            var folding = Fold("[ModuleHeader]", HeaderFolding, context.Start, context.Stop);
            _foldings.Add(folding);
        }
    }

    private IToken? _topOfModuleCommentsStartToken;
    public override void ExitModuleAttributes([NotNull] VBAParser.ModuleAttributesContext context)
    {
        if ((_settings?.FoldModuleAttributes ?? true) && context.ChildCount > 0 && context.Offset.Length > 0)
        {
            var start = context.Start;
            var end = context.Stop;

            var comments = context.GetDescendents<VBAParser.CommentOrAnnotationContext>();
            if (comments.Any())
            {
                var lastAttribute = context
                    .GetDescendents<VBAParser.AttributeStmtContext>()
                    .OrderBy(e => e.Start.StartIndex)
                    .LastOrDefault();

                end = lastAttribute?.Stop ?? context.Stop;

                var firstComment = comments.OrderBy(e => e.Start.StartIndex)
                    .Where(e => e.Start.StartIndex > (end?.StartIndex ?? -1))
                    .FirstOrDefault();

                if (firstComment != null)
                {
                    _topOfModuleCommentsStartToken = firstComment.Start;
                }
            }

            var folding = Fold("[ModuleAttributes]", AttributesFolding, start, end);
            _foldings.Add(folding);
        }
    }

    public override void EnterModuleBodyElement([NotNull] VBAParser.ModuleBodyElementContext context)
    {
        base.EnterModuleBodyElement(context);

        if (_moduleDeclarationsEndToken != null)
        {
            var start = context.Start;

            var declarationsContext = ((VBABaseParserRuleContext)context.Parent.Parent).GetChild<VBAParser.ModuleDeclarationsContext>();
            var declarationComments = declarationsContext.GetDescendents<VBAParser.CommentOrAnnotationContext>()
                .OrderByDescending(e => e.Start.StartIndex)
                .ToLookup(e => e.Start.Line);
            if (declarationComments.Any())
            {
                var mbeStartLine = context.Start.Line;
                var firstCommentLine = declarationComments.Last().First().Start.Line;

                var mbeComments = new List<VBAParser.CommentOrAnnotationContext>();
                for (var currentLine = mbeStartLine - 1; currentLine >= firstCommentLine; currentLine--)
                {
                    var currentLineComments = declarationComments[currentLine];
                    if (currentLineComments.Any())
                    {
                        mbeComments.AddRange(currentLineComments);
                    }
                    else
                    {
                        break;
                    }
                }

                if (mbeComments.Any())
                {
                    start = mbeComments.Last().Start;
                }
            }

            var declarationElements = declarationsContext.GetDescendents<VBABaseParserRuleContext>()
                .Where(e => e.Start.StartIndex < start.StartIndex && !(e is VBAParser.WhiteSpaceContext || e is VBAParser.EndOfStatementContext || e is VBAParser.EndOfLineContext || e is VBAParser.IndividualNonEOFEndOfStatementContext))
                .OrderBy(e => e.Start.StartIndex);

            var lastDeclarationEndToken = declarationElements.LastOrDefault()?.Stop;
            if (lastDeclarationEndToken != null && (_settings?.FoldModuleDeclarations ?? true))
            {
                var folding = Fold("[Declarations]", DeclarationsFolding, start, lastDeclarationEndToken);
                _foldings.Add(folding);
            }
        }
    }

    private IToken? _moduleDeclarationsEndToken;
    public override void ExitModuleDeclarations([NotNull] VBAParser.ModuleDeclarationsContext context)
    {
        if ((_settings?.FoldModuleDeclarations ?? true) && context.ChildCount > 0 && context.Offset.Length > 0)
        {
            _moduleDeclarationsEndToken = _topOfModuleCommentsStartToken != null
                ? _topOfModuleCommentsStartToken
                : context.Stop;
        }
    }

    public override void ExitModuleBodyElement([NotNull] VBAParser.ModuleBodyElementContext context)
    {
        if (context.ChildCount > 0 && context.Offset.Length > 0 && _settings.FoldProcedures)
        {
            var kind = ScopeFolding;

            var sub = context.subStmt();
            var function = context.functionStmt();
            var propGet = context.propertyGetStmt();
            var propLet = context.propertyLetStmt();
            var propSet = context.propertySetStmt();

            if (sub != null)
            {
                _foldings.Add(Fold($"{sub.visibility()?.GetText()} {Tokens.Sub} {sub.subroutineName()?.GetText() ?? string.Empty}()".TrimStart(), kind, sub.Start, sub.Stop));
            }
            else if (function != null)
            {
                _foldings.Add(Fold($"{function.visibility()?.GetText()} {Tokens.Function} {function.functionName()?.GetText() ?? string.Empty}()".TrimStart(), kind, function.Start, function.Stop));
            }
            else if (propGet != null)
            {
                _foldings.Add(Fold($"{propGet.visibility()?.GetText()} {Tokens.Property} {Tokens.Get} {propGet.functionName()?.GetText() ?? string.Empty}()".TrimStart(), kind, propGet.Start, propGet.Stop));
            }
            else if (propLet != null)
            {
                _foldings.Add(Fold($"{propLet.visibility()?.GetText()} {Tokens.Property} {Tokens.Let} {propLet.subroutineName()?.GetText() ?? string.Empty}()".TrimStart(), kind, propLet.Start, propLet.Stop));
            }
            else if (propSet != null)
            {
                _foldings.Add(Fold($"{propSet.visibility()?.GetText()} {Tokens.Property} {Tokens.Set} {propSet.subroutineName()?.GetText() ?? string.Empty}()".TrimStart(), kind, propSet.Start, propSet.Stop));
            }
        }
    }

    private static FoldingRangeKind HeaderFolding { get; } = new("header");
    private static FoldingRangeKind AttributesFolding { get; } = new("attributes");
    private static FoldingRangeKind DeclarationsFolding { get; } = new("declarations");
    private static FoldingRangeKind ScopeFolding { get; } = new("scope");
    private static FoldingRangeKind BlockFolding { get; } = new("block");

    public override void ExitForNextStmt([NotNull] VBAParser.ForNextStmtContext context)
    {
        if (context.ChildCount > 0 && _settings.FoldBlockStatements)
        {
            _foldings.Add(Fold($"For {context.expression(0).GetText()}... Next", BlockFolding, context.Start, context.Stop));
        }
    }

    public override void ExitForEachStmt([NotNull] VBAParser.ForEachStmtContext context)
    {
        if (context.ChildCount > 0 && _settings.FoldBlockStatements)
        {
            _foldings.Add(Fold($"For Each {context.expression(0).GetText()} In {context.expression(1).GetText()}... Next", BlockFolding, context.Start, context.Stop));
        }
    }

    public override void ExitDoLoopStmt([NotNull] VBAParser.DoLoopStmtContext context)
    {
        if (context.ChildCount > 0 && _settings.FoldBlockStatements)
        {
            _foldings.Add(Fold($"Do... Loop", BlockFolding, context.Start, context.Stop));
        }
    }

    public override void ExitWhileWendStmt([NotNull] VBAParser.WhileWendStmtContext context)
    {
        if (context.ChildCount > 0 && _settings.FoldBlockStatements)
        {
            _foldings.Add(Fold($"While {context.expression().GetText()}... Wend", BlockFolding, context.Start, context.Stop));
        }
    }

    public override void ExitIfStmt([NotNull] VBAParser.IfStmtContext context)
    {
        if (context.ChildCount > 0 && _settings.FoldBlockStatements)
        {
            _foldings.Add(Fold($"If {context.booleanExpression().GetText()}... End If", BlockFolding, context.Start, context.Stop));
        }
    }

    public override void ExitSelectCaseStmt([NotNull] VBAParser.SelectCaseStmtContext context)
    {
        if (context.ChildCount > 0 && _settings.FoldBlockStatements)
        {
            _foldings.Add(Fold($"Select Case {context.selectExpression().GetText()}... End Select", BlockFolding, context.Start, context.Stop));
        }
    }

    public override void ExitWithStmt([NotNull] VBAParser.WithStmtContext context)
    {
        if (context.ChildCount > 0 && _settings.FoldBlockStatements)
        {
            _foldings.Add(Fold($"With {context.expression().GetText()}... End With", BlockFolding, context.Start, context.Stop));
        }
    }
}
