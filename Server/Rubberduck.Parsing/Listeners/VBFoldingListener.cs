using Antlr4.Runtime.Misc;
using Rubberduck.InternalApi.Model;
using Rubberduck.Parsing.Grammar;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Listeners;

public class VBFoldingListener : VBAParserBaseListener
{
    private readonly IBlockFoldingSettings _settings;
    private readonly IList<BlockFoldingInfo> _foldings = new List<BlockFoldingInfo>();
    public IEnumerable<BlockFoldingInfo> Foldings => _foldings.OrderBy(e => e.Offset.Start);

    public VBFoldingListener(IBlockFoldingSettings settings)
    {
        _settings = settings;
    }

    public override void EnterModule([NotNull] VBAParser.ModuleContext context)
    {
        _foldings.Clear();
    }

    public override void ExitModuleConfig([NotNull] VBAParser.ModuleConfigContext context)
    {
        if ((_settings?.FoldModuleHeader ?? true) && context.ChildCount > 0 && context.Offset.Length > 0)
        {
            var offset = context.Offset;
            var endToken = context.END().Symbol;

            _foldings.Add(new BlockFoldingInfo($"[ModuleHeader]", new DocumentOffset(0, endToken.StopIndex)));
        }
    }

    private int? _topOfModuleCommentsStartOffset;
    public override void ExitModuleAttributes([NotNull] VBAParser.ModuleAttributesContext context)
    {
        if ((_settings?.FoldModuleAttributes ?? true) && context.ChildCount > 0 && context.Offset.Length > 0)
        {
            var offset = context.Offset;
            var comments = context.GetDescendents<VBAParser.CommentOrAnnotationContext>();
            if (comments.Any())
            {
                var lastAttribute = context.GetDescendents<VBAParser.AttributeStmtContext>()
                    .OrderBy(e => e.Offset.Start).LastOrDefault();

                var end = lastAttribute?.Offset.End ?? -1;

                var firstComment = comments.OrderBy(e => e.Offset.Start)
                    .Where(e => e.Offset.Start > (lastAttribute?.Offset.End ?? -1))
                    .FirstOrDefault();

                if (firstComment != null)
                {
                    _topOfModuleCommentsStartOffset = firstComment.Offset.Start;
                }

                offset = new DocumentOffset(offset.Start, end);
            }

            if (offset.Length > 0)
            {
                _foldings.Add(new BlockFoldingInfo($"[ModuleAttributes]", offset));
            }
        }
    }

    public override void EnterModuleBodyElement([NotNull] VBAParser.ModuleBodyElementContext context)
    {
        base.EnterModuleBodyElement(context);

        if (_moduleDeclarationsOffset.HasValue)
        {
            var startOffset = context.Offset.Start;

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
                    startOffset = mbeComments.Last().Offset.Start;
                }
            }

            var declarationElements = declarationsContext.GetDescendents<VBABaseParserRuleContext>()
                .Where(e => e.Offset.Start < startOffset && !(e is VBAParser.WhiteSpaceContext || e is VBAParser.EndOfStatementContext || e is VBAParser.EndOfLineContext || e is VBAParser.IndividualNonEOFEndOfStatementContext))
                .OrderBy(e => e.Start.StartIndex);

            var lastDeclarationOffset = declarationElements.LastOrDefault()?.Offset.End;

            if (lastDeclarationOffset.HasValue)
            {
                FoldModuleDeclarations(new DocumentOffset(_moduleDeclarationsOffset.Value.Start, lastDeclarationOffset.Value));
            }
        }
    }

    private DocumentOffset? _moduleDeclarationsOffset;

    private void FoldModuleDeclarations(DocumentOffset offset)
    {
        if ((_settings?.FoldModuleDeclarations ?? true) && offset.Length > 0)
        {
            _foldings.Add(new BlockFoldingInfo($"[ModuleDeclarations]", offset));
            _moduleDeclarationsOffset = null;
        }
    }

    public override void ExitModuleDeclarations([NotNull] VBAParser.ModuleDeclarationsContext context)
    {
        if ((_settings?.FoldModuleDeclarations ?? true) && context.ChildCount > 0 && context.Offset.Length > 0)
        {
            var current = context as VBABaseParserRuleContext;
            var offset = current.Offset;

            _moduleDeclarationsOffset = new DocumentOffset(_topOfModuleCommentsStartOffset.HasValue ? _topOfModuleCommentsStartOffset.Value : offset.Start, context.Offset.End - 2);
        }
    }

    public override void ExitModuleBodyElement([NotNull] VBAParser.ModuleBodyElementContext context)
    {
        if (context.ChildCount > 0 && context.Offset.Length > 0 && _settings.FoldProcedures)
        {
            var sub = context.subStmt();
            var function = context.functionStmt();
            var propGet = context.propertyGetStmt();
            var propLet = context.propertyLetStmt();
            var propSet = context.propertySetStmt();

            if (sub != null)
            {
                var offset = sub.Offset;
                _foldings.Add(new BlockFoldingInfo($"{sub.visibility()?.GetText()} {Tokens.Sub} {sub.subroutineName()?.GetText() ?? string.Empty}()".TrimStart(), offset, isDefinition: true));
            }
            else if (function != null)
            {
                var offset = function.Offset;
                _foldings.Add(new BlockFoldingInfo($"{function.visibility()?.GetText()} {Tokens.Function} {function.functionName()?.GetText() ?? string.Empty}()".TrimStart(), offset, isDefinition: true));
            }
            else if (propGet != null)
            {
                var offset = propGet.Offset;
                _foldings.Add(new BlockFoldingInfo($"{propGet.visibility()?.GetText()} {Tokens.Property} {Tokens.Get} {propGet.functionName()?.GetText() ?? string.Empty}()".TrimStart(), offset, isDefinition: true));
            }
            else if (propLet != null)
            {
                var offset = propLet.Offset;
                _foldings.Add(new BlockFoldingInfo($"{propLet.visibility()?.GetText()} {Tokens.Property} {Tokens.Let} {propLet.subroutineName()?.GetText() ?? string.Empty}()".TrimStart(), offset, isDefinition: true));
            }
            else if (propSet != null)
            {
                var offset = propSet.Offset;
                _foldings.Add(new BlockFoldingInfo($"{propSet.visibility()?.GetText()} {Tokens.Property} {Tokens.Set} {propSet.subroutineName()?.GetText() ?? string.Empty}()".TrimStart(), offset, isDefinition: true));
            }
        }
    }

    public override void ExitForNextStmt([NotNull] VBAParser.ForNextStmtContext context)
    {
        if (context.ChildCount > 0 && context.Offset.Length > 0 && _settings.FoldBlockStatements)
        {
            _foldings.Add(new BlockFoldingInfo(
                $"For {context.expression(0).GetText()}...", context.Offset));
        }
    }

    public override void ExitForEachStmt([NotNull] VBAParser.ForEachStmtContext context)
    {
        if (context.ChildCount > 0 && context.Offset.Length > 0 && _settings.FoldBlockStatements)
        {
            _foldings.Add(new BlockFoldingInfo(
                $"For Each {context.expression(0).GetText()} In {context.expression(1).GetText()}...", context.Offset));
        }
    }

    public override void ExitDoLoopStmt([NotNull] VBAParser.DoLoopStmtContext context)
    {
        if (context.ChildCount > 0 && context.Offset.Length > 0 && _settings.FoldBlockStatements)
        {
            _foldings.Add(new BlockFoldingInfo($"Do...", context.Offset));
        }
    }

    public override void ExitWhileWendStmt([NotNull] VBAParser.WhileWendStmtContext context)
    {
        if (context.ChildCount > 0 && context.Offset.Length > 0 && _settings.FoldBlockStatements)
        {
            _foldings.Add(new BlockFoldingInfo($"While {context.expression().GetText()}...", context.Offset));
        }
    }

    public override void ExitIfStmt([NotNull] VBAParser.IfStmtContext context)
    {
        if (context.ChildCount > 0 && context.Offset.Length > 0 && _settings.FoldBlockStatements)
        {
            _foldings.Add(new BlockFoldingInfo($"If {context.booleanExpression().GetText()}...", context.Offset));
        }
    }

    public override void ExitSelectCaseStmt([NotNull] VBAParser.SelectCaseStmtContext context)
    {
        if (context.ChildCount > 0 && context.Offset.Length > 0 && _settings.FoldBlockStatements)
        {
            _foldings.Add(new BlockFoldingInfo($"Select Case {context.selectExpression().GetText()}", context.Offset));
        }
    }

    public override void ExitWithStmt([NotNull] VBAParser.WithStmtContext context)
    {
        if (context.ChildCount > 0 && context.Offset.Length > 0 && _settings.FoldBlockStatements)
        {
            _foldings.Add(new BlockFoldingInfo($"With {context.expression().GetText()}...", context.Offset));
        }
    }
}
