using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using Rubberduck.Parsing.Grammar;
using Rubberduck.Parsing.Model;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Runtime;

namespace Rubberduck.Parsing.Listeners
{
    public class SemanticHighlighting : DocumentColorizingTransformer
    {
        private readonly VBABaseParserRuleContext _context;

        public SemanticHighlighting(VBABaseParserRuleContext context)
        {
            _context = context;
        }

        protected override void ColorizeLine(DocumentLine line)
        {
            // TODO
        }
    }

    public interface IBlockCompletionSettings
    {
        bool CompleteScope { get; }
        bool CompleteBlock { get; }
    }

    public class BlockCompletionInfo
    {
        public BlockCompletionInfo(VBABaseParserRuleContext context, string missingToken) 
        {
            Context = context;
            MissingToken = missingToken;
        }

        public VBABaseParserRuleContext Context { get; }
        public string MissingToken { get; }
    }

    public class BlockCompletionListener : VBAParserBaseListener
    {
        private readonly IBlockCompletionSettings _settings;
        private readonly IList<BlockCompletionInfo> _info = new List<BlockCompletionInfo>();

        public BlockCompletionListener(IBlockCompletionSettings settings)
        {
            _settings = settings;
        }

        public IEnumerable<BlockCompletionInfo> BlockCompletionInfos => _info;

        public override void ExitModuleBodyElement([NotNull] VBAParser.ModuleBodyElementContext context)
        {
            if (_settings.CompleteScope)
            {
                var sub = context.subStmt();
                var function = context.functionStmt();
                var propGet = context.propertyGetStmt();
                var propLet = context.propertyLetStmt();
                var propSet = context.propertySetStmt();

                if (sub != null)
                {
                    if (sub.END_SUB() is null)
                    {
                        _info.Add(new BlockCompletionInfo(sub, Tokens.EndSub));
                    }
                }
                else if (function != null)
                {
                    if (function.END_FUNCTION() is null)
                    {
                        _info.Add(new BlockCompletionInfo(function, Tokens.EndFunction));
                    }
                }
                else if (propGet != null)
                {
                    if (propGet.END_PROPERTY() is null)
                    {
                        _info.Add(new BlockCompletionInfo(propGet, Tokens.EndProperty));
                    }
                }
                else if (propLet != null)
                {
                    if (propLet.END_PROPERTY() is null)
                    {
                        _info.Add(new BlockCompletionInfo(propLet, Tokens.EndProperty));
                    }
                }
                else if (propSet != null)
                {
                    if (propSet.END_PROPERTY() is null)
                    {
                        _info.Add(new BlockCompletionInfo(propSet, Tokens.EndProperty));
                    }
                }
            }
        }

        public override void ExitForNextStmt([NotNull] VBAParser.ForNextStmtContext context)
        {
            if (_settings.CompleteBlock && context.ChildCount > 0 && context.Offset.Length > 0)
            {
                if (context.NEXT() is null)
                {
                    _info.Add(new BlockCompletionInfo(context, Tokens.Next));
                }
            }
        }

        public override void ExitForEachStmt([NotNull] VBAParser.ForEachStmtContext context)
        {
            if (_settings.CompleteBlock && context.ChildCount > 0 && context.Offset.Length > 0)
            {
                if (context.NEXT() is null)
                {
                    _info.Add(new BlockCompletionInfo(context, Tokens.Next));
                }
            }
        }

        public override void ExitDoLoopStmt([NotNull] VBAParser.DoLoopStmtContext context)
        {
            if (_settings.CompleteBlock && context.ChildCount > 0 && context.Offset.Length > 0)
            {
                if (context.LOOP() is null)
                {
                    _info.Add(new BlockCompletionInfo(context, Tokens.Loop));
                }
            }
        }

        public override void ExitWhileWendStmt([NotNull] VBAParser.WhileWendStmtContext context)
        {
            if (_settings.CompleteBlock && context.ChildCount > 0 && context.Offset.Length > 0)
            {
                if (context.WEND() is null)
                {
                    _info.Add(new BlockCompletionInfo(context, Tokens.Wend));
                }
            }
        }

        public override void ExitIfStmt([NotNull] VBAParser.IfStmtContext context)
        {
            if (_settings.CompleteBlock && context.ChildCount > 0 && context.Offset.Length > 0)
            {
                if (context.END_IF() is null)
                {
                    _info.Add(new BlockCompletionInfo(context, Tokens.EndIf));
                }
            }
        }

        public override void ExitSelectCaseStmt([NotNull] VBAParser.SelectCaseStmtContext context)
        {
            if (_settings.CompleteBlock && context.ChildCount > 0 && context.Offset.Length > 0)
            {
                if (context.END_SELECT() is null)
                {
                    _info.Add(new BlockCompletionInfo(context, Tokens.EndSelect));
                }
            }
        }

        public override void ExitWithStmt([NotNull] VBAParser.WithStmtContext context)
        {
            if (_settings.CompleteBlock && context.ChildCount > 0 && context.Offset.Length > 0)
            {
                if (context.END_WITH() is null)
                {
                    _info.Add(new BlockCompletionInfo(context, Tokens.EndWith));
                }
            }
        }
    }
}
