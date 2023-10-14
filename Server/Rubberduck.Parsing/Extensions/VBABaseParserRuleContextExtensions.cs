using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Rubberduck.Parsing.Grammar;
using Antlr4.Runtime.Misc;

namespace Rubberduck.Parsing;

public static class VBABaseParserRuleContextExtensions
{        
    /// <summary>
    ///  Returns whether the other context from the same parse tree is wholly contained in the current context
    /// </summary>
    public static bool Contains(this VBABaseParserRuleContext context, VBABaseParserRuleContext otherContextInSameParseTree)
    {
        if (context is null || otherContextInSameParseTree is null)
        {
            return false;
        }

        return context.start.TokenIndex <= otherContextInSameParseTree.start.TokenIndex
               && context.stop.TokenIndex >= otherContextInSameParseTree.stop.TokenIndex;
    }

    /// <summary>
    ///  Gets the tokens belonging to the context from the token stream.
    /// </summary>
    public static IEnumerable<IToken> GetTokens(this VBABaseParserRuleContext context, CommonTokenStream tokenStream)
    {
        var sourceInterval = context?.SourceInterval ?? Interval.Invalid;
        if (sourceInterval.Equals(Interval.Invalid) || sourceInterval.b < sourceInterval.a)
        {
            return new List<IToken>();
        }
        return tokenStream.GetTokens(sourceInterval.a, sourceInterval.b);
    }

    /// <summary>
    ///  Gets the original source, without "synthetic" text such as an 'EOF' token.
    /// </summary>
    public static string GetText(this VBABaseParserRuleContext context, ICharStream stream)
    {
        // Can be null if the input is empty it seems.
        if (context?.Stop is null)
        {
            return string.Empty;
        }
        return stream.GetText(new Interval(context.Start.StartIndex, context.Stop.StopIndex));
    }

    /// <summary>
    /// Returns the first direct child of 'context' that is of the generic Type.
    /// </summary>
    public static TContext GetChild<TContext>(this VBABaseParserRuleContext context) where TContext : VBABaseParserRuleContext
    {
        if (context is null)
        {
            return default;
        }

        return context.children.OfType<TContext>().FirstOrDefault();
    }

    /// <summary>
    /// Determines if any of the context's ancestors are the generic Type.
    /// </summary>
    public static bool IsDescendentOf<TContext>(this VBABaseParserRuleContext context) where TContext: VBABaseParserRuleContext
    {
        if (context is null)
        {
            return false;
        }

        if (context is TContext)
        {
            return GetAncestor_Recursive<TContext>((VBABaseParserRuleContext)context.Parent) != null;
        }
        return GetAncestor_Recursive<TContext>(context) != null;
    }

    /// <summary>
    /// Determines if any of the context's ancestors are equal to the parameter 'ancestor'.
    /// </summary>
    public static bool IsDescendentOf<T>(this VBABaseParserRuleContext context, T ancestor) where T : VBABaseParserRuleContext
    {
        if (context is null || ancestor is null)
        {
            return false;
        }
        if (context == ancestor)
        {
            return IsDescendentOf_Recursive(context.Parent, ancestor);
        }
        return IsDescendentOf_Recursive(context, ancestor);
    }

    private static bool IsDescendentOf_Recursive(IParseTree context, IParseTree targetParent)
    {
        if (context is null)
        {
            return false;
        }
        return context == targetParent 
               || IsDescendentOf_Recursive(context.Parent, targetParent);
    }

    /// <summary>
    /// Returns the context's first ancestor of the generic Type.
    /// </summary>
    public static TContext GetAncestor<TContext>(this VBABaseParserRuleContext context) where TContext: VBABaseParserRuleContext
    {
        switch (context)
        {
            case null:
                return default;
            case TContext _:
                return GetAncestor_Recursive<TContext>((VBABaseParserRuleContext)context.Parent);
            default:
                return GetAncestor_Recursive<TContext>(context);
        }
    }

    /// <summary>
    /// Tries to return the context's first ancestor of the generic Type.
    /// </summary>
    public static bool TryGetAncestor<TContext>(this VBABaseParserRuleContext context, out TContext ancestor) where TContext : VBABaseParserRuleContext
    {
        ancestor = context.GetAncestor<TContext>();
        return ancestor != null;
    }

    private static TContext GetAncestor_Recursive<TContext>(VBABaseParserRuleContext context) where TContext: VBABaseParserRuleContext
    {
        switch (context)
        {
            case null:
                return default;
            case TContext tContext:
                return tContext;
            default:
                return GetAncestor_Recursive<TContext>((VBABaseParserRuleContext)context.Parent);
        }
    }

    /// <summary>
    /// Returns the context's first ancestor containing the token with the specified token index or the context itels if it already contains the token.
    /// </summary>
    public static VBABaseParserRuleContext GetAncestorContainingTokenIndex(this VBABaseParserRuleContext context, int tokenIndex)
    {
        if (context is null)
        {
            return default;
        }

        if (context.ContainsTokenIndex(tokenIndex))
        {
            return context;
        }

        if (!(context.Parent is VBABaseParserRuleContext parent))
        {
            return default;
        }

        return GetAncestorContainingTokenIndex(parent, tokenIndex);
    }

    /// <summary>
    /// Determines whether the context contains the token with the specified token index.
    /// </summary>
    public static bool ContainsTokenIndex(this VBABaseParserRuleContext context, int tokenIndex)
    {
        if (context?.Stop is null)
        {
            return false;
        }

        return context.Start.TokenIndex <= tokenIndex && tokenIndex <= context.Stop.TokenIndex;
    }

    /// <summary>
    /// Determines whether the context contains the token with the specified token index.
    /// </summary>
    public static bool ContainsOffset(this VBABaseParserRuleContext context, int offset)
    {
        if (context?.Stop is null || context.Stop.StopIndex < context.Start.StartIndex)
        {
            return false;
        }

        return context.Start.StartIndex <= offset && offset <= context.Stop.StopIndex;
    }
    
    /// <summary>
    /// Returns the context's first descendent of the generic Type.
    /// </summary>
    public static TContext GetDescendent<TContext>(this VBABaseParserRuleContext context) where TContext : VBABaseParserRuleContext
    {
        if (context?.children is null)
        {
            return null;
        }

        foreach (var child in context.children)
        {
            if (child is null)
            {
                continue;
            }

            if (child is TContext match)
            {
                return match;
            }
            
            var childResult = (child as VBABaseParserRuleContext)?.GetDescendent<TContext>();
            if (childResult != null)
            {
                return childResult;
            }
        }

        return null;
    }

    /// <summary>
    /// Returns all the context's descendents of the generic Type.
    /// </summary>
    public static IEnumerable<TContext> GetDescendents<TContext>(this VBABaseParserRuleContext context) where TContext : VBABaseParserRuleContext
    {
        var walker = new ParseTreeWalker();
        var listener = new ChildNodeListener<TContext>();
        walker.Walk(listener, context);
        return listener.Matches;
    }

    /// <summary>
    /// Try to get the first child of the generic context type.
    /// </summary>
    public static bool TryGetChildContext<TContext>(this VBABaseParserRuleContext ctxt, out TContext opCtxt) where TContext : VBABaseParserRuleContext
    {
        opCtxt = ctxt.GetChild<TContext>();
        return opCtxt != null;
    }

    /// <summary>
    /// Returns the endOfStatementContext's first endOfLine context.
    /// </summary>
    public static VBAParser.EndOfLineContext GetFirstEndOfLine(this VBAParser.EndOfStatementContext endOfStatement)
    {
        //This dedicated method exists for performance reasons on hot-paths.
        var individualEndOfStatements = endOfStatement?.individualNonEOFEndOfStatement();

        if (individualEndOfStatements is null)
        {
            return null;
        }

        foreach (var individualEndOfStatement in individualEndOfStatements)
        {
            var endOfLine = individualEndOfStatement.endOfLine();
            if (endOfLine != null)
            {
                return endOfLine;
            }
        }
        //The only remaining alternative is whitespace followed by an EOF.
        return null;
    }

    /// <summary>
    /// Determines if the context's module declares or defaults to 
    /// Option Compare Binary 
    /// </summary>
    public static bool IsOptionCompareBinary(this VBABaseParserRuleContext context)
    {
        if( !(context is VBAParser.ModuleContext moduleContext))
        {
            moduleContext = context.GetAncestor<VBAParser.ModuleContext>();
            if (moduleContext is null)
            {
                throw new ArgumentException($"Unable to obtain a VBAParser.ModuleContext reference from 'context'");
            }
        }

        var optionContext = moduleContext.GetDescendent<VBAParser.OptionCompareStmtContext>();
        return (optionContext is null) || !(optionContext.BINARY() is null);
    }

    /// <summary>
    /// Returns the context's widest descendent of the generic type containing the token with the specified token index.
    /// </summary>
    public static TContext GetWidestDescendentContainingTokenIndex<TContext>(this VBABaseParserRuleContext context, int tokenIndex) where TContext : VBABaseParserRuleContext
    {
        var descendents = GetDescendentsContainingTokenIndex<TContext>(context, tokenIndex);
        return descendents.FirstOrDefault();
    }

    /// <summary>
    /// Returns the context's smallest descendent of the generic type containing the token with the specified token index.
    /// </summary>
    public static TContext GetSmallestDescendentContainingTokenIndex<TContext>(this VBABaseParserRuleContext context, int tokenIndex) where TContext : VBABaseParserRuleContext
    {
        var descendents = GetDescendentsContainingTokenIndex<TContext>(context, tokenIndex);
        return descendents.LastOrDefault();
    }

    /// <summary>
    /// Returns all the context's descendents of the generic type containing the token with the specified token index.
    /// If there are multiple matches, they are ordered from outermost to innermost context.
    /// </summary>
    public static IEnumerable<TContext> GetDescendentsContainingTokenIndex<TContext>(this VBABaseParserRuleContext context, int tokenIndex) where TContext : VBABaseParserRuleContext
    {
        if (context is null || !context.ContainsTokenIndex(tokenIndex))
        {
            return new List<TContext>();
        }

        var matches = new List<TContext>();
        if (context is TContext match)
        {
            matches.Add(match);
        }

        foreach (var child in context.children)
        {
            if (child is VBABaseParserRuleContext childContext && childContext.ContainsTokenIndex(tokenIndex))
            {
                matches.AddRange(childContext.GetDescendentsContainingTokenIndex<TContext>(tokenIndex));
                break;  //Only one child can contain the token index.
            }
        }

        return matches;
    }

    /// <summary>
    /// Returns all the context's descendents of the generic type containing the specified document offset.
    /// If there are multiple matches, they are ordered from outermost to innermost context.
    /// </summary>
    public static IEnumerable<TContext> GetDescendentsContainingOffset<TContext>(this VBABaseParserRuleContext context, int offset) where TContext : VBABaseParserRuleContext
    {
        if (context is null || !context.ContainsOffset(offset))
        {
            return new List<TContext>();
        }

        var matches = new List<TContext>();
        if (context is TContext match)
        {
            matches.Add(match);
        }

        foreach (var child in context.children)
        {
            if (child is VBABaseParserRuleContext childContext && childContext.ContainsOffset(offset))
            {
                matches.AddRange(childContext.GetDescendentsContainingOffset<TContext>(offset));
                break;  //Only one child can contain the offset.
            }
        }

        return matches;
    }

    /// <summary>
    /// Returns the context containing the token preceding the context provided it is of the specified generic type.
    /// </summary>
    public static bool TryGetPrecedingContext<TContext>(this VBABaseParserRuleContext context, out TContext precedingContext) where TContext : VBABaseParserRuleContext
    {
        precedingContext = null;
        if (context is null)
        {
            return false;
        }

        var precedingTokenIndex = context.Start.TokenIndex - 1;
        var ancestorContainingPrecedingIndex = context.GetAncestorContainingTokenIndex(precedingTokenIndex);

        if (ancestorContainingPrecedingIndex is null)
        {
            return false;
        }

        precedingContext = ancestorContainingPrecedingIndex.GetWidestDescendentContainingTokenIndex<TContext>(precedingTokenIndex);
        return precedingContext != null;
    }

    /// <summary>
    /// Returns the context containing the token following the context provided it is of the specified generic type.
    /// </summary>
    public static bool TryGetFollowingContext<TContext>(this VBABaseParserRuleContext context, out TContext followingContext) where TContext : VBABaseParserRuleContext
    {
        followingContext = null;
        if (context is null)
        {
            return false;
        }

        var followingTokenIndex = context.Stop.TokenIndex + 1;
        var ancestorContainingFollowingIndex = context.GetAncestorContainingTokenIndex(followingTokenIndex);

        if (ancestorContainingFollowingIndex is null)
        {
            return false;
        }

        followingContext = ancestorContainingFollowingIndex.GetWidestDescendentContainingTokenIndex<TContext>(followingTokenIndex);
        return followingContext != null;
    }

    /// <summary>
    /// Checks a block of code for executable statments and returns true if are present.
    /// </summary>
    /// <param name="block">The block to inspect</param>
    /// <param name="considerAllocations">Determines wheather Dim or Const statements should be considered executables</param>
    /// <returns></returns>
    public static bool ContainsExecutableStatements(this VBAParser.BlockContext block, bool considerAllocations = false)
    {
        return block?.children != null && ContainsExecutableStatements(block.children, considerAllocations);
    }

    private static bool ContainsExecutableStatements(
        IList<IParseTree> blockChildren,
        bool considerAllocations = false)
    {
        foreach (var child in blockChildren)
        {
            if (child is VBAParser.BlockStmtContext blockStmt)
            {
                var mainBlockStmt = blockStmt.mainBlockStmt();

                if (mainBlockStmt is null)
                {
                    continue;   //We have a lone line label, which is not executable.
                }

                // if inspection does not consider allocations as executables,
                // exclude variables and consts because they are not executable statements
                if (!considerAllocations && IsConstOrVariable(mainBlockStmt.GetChild(0)))
                {
                    continue;
                }

                return true;
            }

            if (child is VBAParser.RemCommentContext ||
                child is VBAParser.CommentContext ||
                child is VBAParser.CommentOrAnnotationContext ||
                child is VBAParser.EndOfStatementContext)
            {
                continue;
            }

            return true;
        }

        return false;
    }

    private static bool IsConstOrVariable(IParseTree block)
    {
        return block is VBAParser.VariableStmtContext || block is VBAParser.ConstStmtContext;
    }

    private class ChildNodeListener<TContext> : VBAParserBaseListener where TContext : VBABaseParserRuleContext
    {
        private readonly HashSet<TContext> _matches = new HashSet<TContext>();
        public IEnumerable<TContext> Matches => _matches;

        public override void EnterEveryRule(ParserRuleContext context)
        {
            if (context is TContext match)
            {
                _matches.Add(match);
            }
        }
    }
}