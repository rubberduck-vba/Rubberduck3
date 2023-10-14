using Antlr4.Runtime.Misc;
using Rubberduck.Parsing.Grammar;

namespace Rubberduck.Parsing.Listeners;

internal class CommentListener : VBAParserBaseListener
{
    private readonly IList<VBAParser.RemCommentContext> _remComments = new List<VBAParser.RemCommentContext>();
    public IEnumerable<VBAParser.RemCommentContext> RemComments => _remComments;

    private readonly IList<VBAParser.CommentContext> _comments = new List<VBAParser.CommentContext>();
    public IEnumerable<VBAParser.CommentContext> Comments => _comments;

    public override void ExitRemComment([NotNull] VBAParser.RemCommentContext context)
    {
        _remComments.Add(context);
    }

    public override void ExitComment([NotNull] VBAParser.CommentContext context)
    {
        _comments.Add(context);
    }
}
