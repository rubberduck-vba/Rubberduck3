using Antlr4.Runtime.Misc;
using Rubberduck.Parsing.Grammar;
using Rubberduck.Parsing.Model;
using System.Collections.Generic;

namespace Rubberduck.Parsing.Listeners
{
    public class VBFoldingListener : VBAParserBaseListener
    {
        private readonly IList<BlockFoldingInfo> _foldings = new List<BlockFoldingInfo>();
        public IEnumerable<BlockFoldingInfo> Foldings => _foldings;

        #region procedures
        public override void ExitSubStmt([NotNull] VBAParser.SubStmtContext context)
        {
            _foldings.Add(new BlockFoldingInfo(
                context.subroutineName()?.GetText()?? string.Empty,
                context.Offset,
                isDefinition: true));
        }

        public override void ExitFunctionStmt([NotNull] VBAParser.FunctionStmtContext context)
        {
            _foldings.Add(new BlockFoldingInfo(
                context.functionName()?.GetText()?? string.Empty,
                context.Offset,
                isDefinition: true));
        }

        public override void ExitPropertyGetStmt([NotNull] VBAParser.PropertyGetStmtContext context)
        {
            _foldings.Add(new BlockFoldingInfo(
                context.functionName()?.GetText()?? string.Empty,
                context.Offset,
                isDefinition: true));
        }

        public override void ExitPropertyLetStmt([NotNull] VBAParser.PropertyLetStmtContext context)
        {
            _foldings.Add(new BlockFoldingInfo(
                context.subroutineName()?.GetText()?? string.Empty,
                context.Offset,
                isDefinition: true));
        }

        public override void ExitPropertySetStmt([NotNull] VBAParser.PropertySetStmtContext context)
        {
            _foldings.Add(new BlockFoldingInfo(
                context.subroutineName()?.GetText()?? string.Empty,
                context.Offset,
                isDefinition: true));
        }
        #endregion

        #region loops
        public override void ExitForNextStmt([NotNull] VBAParser.ForNextStmtContext context)
        {
            _foldings.Add(new BlockFoldingInfo(
                $"For {context.expression(0).GetText()}...", context.Offset));
        }

        public override void ExitForEachStmt([NotNull] VBAParser.ForEachStmtContext context)
        {
            _foldings.Add(new BlockFoldingInfo(
                $"For Each {context.expression(0).GetText()} In {context.expression(1).GetText()}...", context.Offset));
        }

        public override void ExitDoLoopStmt([NotNull] VBAParser.DoLoopStmtContext context)
        {
            _foldings.Add(new BlockFoldingInfo($"Do...", context.Offset));
        }

        public override void ExitWhileWendStmt([NotNull] VBAParser.WhileWendStmtContext context)
        {
            _foldings.Add(new BlockFoldingInfo($"While {context.expression().GetText()}...", context.Offset));
        }
        #endregion

        public override void ExitIfStmt([NotNull] VBAParser.IfStmtContext context)
        {
            _foldings.Add(new BlockFoldingInfo($"If {context.booleanExpression().GetText()}...", context.Offset));
        }

        public override void ExitSelectCaseStmt([NotNull] VBAParser.SelectCaseStmtContext context)
        {
            _foldings.Add(new BlockFoldingInfo($"Select Case {context.selectExpression().GetText()}", context.Offset));
        }

        public override void ExitUdtDeclaration([NotNull] VBAParser.UdtDeclarationContext context)
        {
            _foldings.Add(new BlockFoldingInfo(
                context.untypedIdentifier().GetText(),
                context.Offset,
                isDefinition: true));
        }

        public override void ExitEnumerationStmt([NotNull] VBAParser.EnumerationStmtContext context)
        {
            _foldings.Add(new BlockFoldingInfo(
                context.identifier().GetText(),
                context.Offset,
                isDefinition: true));
        }

        public override void ExitWithStmt([NotNull] VBAParser.WithStmtContext context)
        {
            _foldings.Add(new BlockFoldingInfo($"With {context.expression().GetText()}...", context.Offset));
        }
    }
}
