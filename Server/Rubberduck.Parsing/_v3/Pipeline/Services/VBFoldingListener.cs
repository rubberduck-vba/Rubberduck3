using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Model;
using Rubberduck.InternalApi.Settings.Model.Editor.CodeFolding;
using Rubberduck.Parsing.Grammar;
using System.Net.Mime;

namespace Rubberduck.Parsing._v3.Pipeline.Services;

public class VBFoldingListener : VBAParserBaseListener
{
    private readonly CodeFoldingSettings _settings;
    private readonly List<FoldingRange> _foldings = [];
    public IEnumerable<FoldingRange> Foldings => _foldings.OrderBy(e => e.StartLine);

    public VBFoldingListener(CodeFoldingSettings settings)
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
            StartCharacter = start.Column,
            EndCharacter = end.EndColumn(),
        };
    }

    private bool CanFold(string key)
    {
        return true;
        return key switch
        {
            nameof(FoldScopesSetting) => _settings.FoldScopes,
            nameof(FoldBlockStatementsSetting) => _settings.FoldBlockStatements,
            nameof(FoldModuleHeaderSetting) => _settings.FoldModuleHeader,
            nameof(FoldModuleDeclarationsSetting) => _settings.FoldModuleDeclarations,
            nameof(FoldModuleAttributesSetting) => _settings.FoldModuleAttributes,
            nameof(FoldRegionsSetting) => _settings.FoldRegions,
            _ => false,
        };
    }

    public override void EnterModule([NotNull] VBAParser.ModuleContext context)
    {
        _foldings.Clear();
    }

    private VBAParser.ModuleHeaderContext _headerContext;
    public override void ExitModuleHeader([NotNull] VBAParser.ModuleHeaderContext context)
    {
        _headerContext = context;
    }

    public override void ExitModuleConfig([NotNull] VBAParser.ModuleConfigContext context)
    {
        if (CanFold(nameof(FoldModuleHeaderSetting)))
        {
            if (context.ChildCount > 0 && _headerContext != null)
            {
                _foldings.Add(Fold("[ModuleHeader]", HeaderFolding, _headerContext.Start, context.Stop));
            }
        }
    }

    private IToken? _topOfModuleCommentsStartToken;
    public override void ExitModuleAttributes([NotNull] VBAParser.ModuleAttributesContext context)
    {
        if (CanFold(nameof(FoldModuleAttributesSetting)))
        {

            if (context.ChildCount > 0)
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
    }

    public override void EnterModuleBodyElement([NotNull] VBAParser.ModuleBodyElementContext context)
    {
        if (CanFold(nameof(FoldModuleDeclarationsSetting)))
        {
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
                if (lastDeclarationEndToken != null)
                {
                    var folding = Fold("[Declarations]", DeclarationsFolding, declarationsContext.Start, lastDeclarationEndToken);
                    _foldings.Add(folding);
                }
            }
        }
    }

    private IToken? _moduleDeclarationsEndToken;
    public override void ExitModuleDeclarations([NotNull] VBAParser.ModuleDeclarationsContext context)
    {
        if (CanFold(nameof(FoldModuleDeclarationsSetting)))
        {
            if (context.ChildCount > 0)
            {
                _moduleDeclarationsEndToken = _topOfModuleCommentsStartToken != null
                    ? _topOfModuleCommentsStartToken
                    : context.Stop;
            }
        }
    }

    public override void ExitEnumerationStmt([NotNull] VBAParser.EnumerationStmtContext context)
    {
        if (CanFold(nameof(FoldBlockStatementsSetting))) // TODO FoldEnumStatements?
        {
            _foldings.Add(Fold($"{Tokens.Enum} {context.identifier().GetText()}...", EnumFolding, context.Start, context.Stop));
        }
    }

    public override void ExitUdtDeclaration([NotNull] VBAParser.UdtDeclarationContext context)
    {
        if (CanFold(nameof(FoldBlockStatementsSetting))) // TODO FoldUdtStatements?
        {
            var visibility = context.visibility()?.GetText();
            var name = context.untypedIdentifier().GetText();
            if (visibility != null)
            {
                _foldings.Add(Fold($"{visibility} {Tokens.Type} {name}...", UserDefinedTypeFolding, context.Start, context.Stop));
            }
        }
    }

    public override void ExitModuleBodyElement([NotNull] VBAParser.ModuleBodyElementContext context)
    {
        if (CanFold(nameof(FoldScopesSetting)))
        {
            if (context.ChildCount > 0)
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
    }

    private static FoldingRangeKind HeaderFolding { get; } = new("header");
    private static FoldingRangeKind AttributesFolding { get; } = new("attributes");
    private static FoldingRangeKind DeclarationsFolding { get; } = new("declarations");
    private static FoldingRangeKind EnumFolding { get; } = new("enum");
    private static FoldingRangeKind UserDefinedTypeFolding { get; } = new("udt");
    private static FoldingRangeKind ScopeFolding { get; } = new("scope");
    private static FoldingRangeKind BlockFolding { get; } = new("block");

    public override void ExitForNextStmt([NotNull] VBAParser.ForNextStmtContext context)
    {
        if (CanFold(nameof(FoldBlockStatementsSetting)))
        {
            if (context.ChildCount > 0)
            {
                _foldings.Add(Fold($"For {context.expression(0).GetText()}... Next", BlockFolding, context.Start, context.NEXT().Symbol));
            }
        }
    }

    public override void ExitForEachStmt([NotNull] VBAParser.ForEachStmtContext context)
    {
        if (CanFold(nameof(FoldBlockStatementsSetting)))
        {
            if (context.ChildCount > 0)
            {
                _foldings.Add(Fold($"For Each {context.expression(0).GetText()} In {context.expression(1).GetText()}... Next", BlockFolding, context.Start, context.NEXT().Symbol));
            }
        }
    }

    public override void ExitDoLoopStmt([NotNull] VBAParser.DoLoopStmtContext context)
    {
        if (CanFold(nameof(FoldBlockStatementsSetting)))
        {
            if (context.ChildCount > 0)
            {
                _foldings.Add(Fold($"Do... Loop", BlockFolding, context.Start, context.LOOP().Symbol));
            }
        }
    }

    public override void ExitWhileWendStmt([NotNull] VBAParser.WhileWendStmtContext context)
    {
        if (CanFold(nameof(FoldBlockStatementsSetting)))
        {
            if (context.ChildCount > 0)
            {
                _foldings.Add(Fold($"While {context.expression().GetText()}... Wend", BlockFolding, context.Start, context.WEND().Symbol));
            }
        }
    }

    public override void ExitIfStmt([NotNull] VBAParser.IfStmtContext context)
    {
        if (CanFold(nameof(FoldBlockStatementsSetting)))
        {
            if (context.ChildCount > 0)
            {
                _foldings.Add(Fold($"If {context.booleanExpression().GetText()}... End If", BlockFolding, context.Start, context.END_IF().Symbol));
            }
        }
    }

    public override void ExitElseIfBlock([NotNull] VBAParser.ElseIfBlockContext context)
    {
        if (CanFold(nameof(FoldBlockStatementsSetting)))
        {
            if (context.ChildCount > 0)
            {
                var lastToken = context.GetDescendents<VBAParser.BlockStmtContext>().OrderBy(e => e.Start.StartIndex).LastOrDefault()?.Stop
                    ?? context.Stop;

                _foldings.Add(Fold($"Else If...", BlockFolding, context.Start, lastToken));
            }
        }
    }

    public override void ExitElseBlock([NotNull] VBAParser.ElseBlockContext context)
    {
        if (CanFold(nameof(FoldBlockStatementsSetting)))
        {
            if (context.ChildCount > 0)
            {
                var lastToken = context.GetDescendents<VBAParser.BlockStmtContext>().OrderBy(e => e.Start.StartIndex).LastOrDefault()?.Stop
                    ?? context.Stop;

                _foldings.Add(Fold($"Else...", BlockFolding, context.Start, lastToken));
            }
        }
    }

    public override void ExitSelectCaseStmt([NotNull] VBAParser.SelectCaseStmtContext context)
    {
        if (CanFold(nameof(FoldBlockStatementsSetting)))
        {
            if (context.ChildCount > 0)
            {
                _foldings.Add(Fold($"Select Case {context.selectExpression().GetText()}... End Select", BlockFolding, context.Start, context.Stop));
            }
        }
    }

    public override void ExitWithStmt([NotNull] VBAParser.WithStmtContext context)
    {
        if (CanFold(nameof(FoldBlockStatementsSetting)))
        {
            if (context.ChildCount > 0)
            {
                _foldings.Add(Fold($"With {context.expression().GetText()}... End With", BlockFolding, context.Start, context.Stop));
            }
        }
    }
}
