using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Rubberduck.InternalApi.Model;
using Rubberduck.InternalApi.Model.Declarations;
//using Rubberduck.Parsing.Model.Symbols;

namespace Rubberduck.Parsing.Model;

public class ModuleParseResults
{
    private readonly IDictionary<CodeKind, (IParseTree tree, ITokenStream tokens)> _parseTrees;

    public ModuleParseResults(
        IDictionary<CodeKind, (IParseTree tree, ITokenStream tokens)> parseTrees,
        int contentHash,
        //IEnumerable<CommentNode> comments,
        IEnumerable<IParseTreeAnnotation> annotations,
        LogicalLineStore logicalLines,
        IDictionary<(string scopeIdentifier, DeclarationType scopeType), Attributes> attributes,
        IDictionary<(string scopeIdentifier, DeclarationType scopeType), ParserRuleContext> membersAllowingAttributes)
    {
        _parseTrees = parseTrees;

        ContentHash = contentHash;
        //Comments = comments;
        Annotations = annotations;
        Attributes = attributes;
        MembersAllowingAttributes = membersAllowingAttributes;
        LogicalLines = logicalLines;
    }

    public (IParseTree? tree, ITokenStream? tokens) this[CodeKind codeKind]
    {
        get => (_parseTrees?.TryGetValue(codeKind, out var result) ?? false) ? result : (null, null);
    }

    public int ContentHash { get; }
    //public IEnumerable<CommentNode> Comments { get; }
    public IEnumerable<IParseTreeAnnotation> Annotations { get; }
    public LogicalLineStore LogicalLines { get; }
    public IDictionary<(string scopeIdentifier, DeclarationType scopeType), Attributes> Attributes { get; }
    public IDictionary<(string scopeIdentifier, DeclarationType scopeType), ParserRuleContext> MembersAllowingAttributes { get; }
}
