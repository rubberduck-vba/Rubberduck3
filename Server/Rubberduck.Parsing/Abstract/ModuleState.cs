using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Rubberduck.Parsing.Model;
using Rubberduck.Parsing.Model.Symbols;
using Rubberduck.Unmanaged.Model;

namespace Rubberduck.Parsing.VBA.Parsing;

public sealed class ModuleState : IEquatable<ModuleState> 
{
    public ModuleState(QualifiedModuleName module, ModuleParseResults results, ParserState status = ParserState.Pending)
    {
        Module = module;
        ContentHash = results?.ContentHash ?? 0;
        LogicalLines = results?.LogicalLines ?? new LogicalLineStore(Enumerable.Empty<int>());
        Comments = results?.Comments ?? Enumerable.Empty<CommentNode>();

        (ParseTree, TokenStream) = results?[CodeKind.RubberduckEditorModule] ?? (null, null);

        IsNew = status == ParserState.Pending;
        IsModified = false;
        Status = status;
    }
    
    public QualifiedModuleName Module { get; }

    public int ContentHash { get; }
    public bool IsNew { get; }
    public bool IsModified { get; }

    public LogicalLineStore LogicalLines { get; }
    public IEnumerable<CommentNode> Comments { get; }

    public ITokenStream TokenStream { get; }
    public IParseTree ParseTree { get; }

    public ParserState Status { get; internal set; }

    public bool Equals(ModuleState other)
    {
        if (other is null) { return false; }
        if (ReferenceEquals(this, other)) { return true; }

        return other.Module.Equals(Module);
    }

    public override bool Equals(object obj) => Equals(obj as ModuleState);
    public override int GetHashCode() => Module.GetHashCode();
}
