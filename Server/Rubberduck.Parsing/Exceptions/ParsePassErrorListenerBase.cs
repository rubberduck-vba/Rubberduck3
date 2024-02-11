using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Exceptions;

public class ParsePassErrorListenerBase : RubberduckParseErrorListenerBase
{
    public ParsePassErrorListenerBase(WorkspaceFileUri uri, CodeKind codeKind) 
        : base(uri, codeKind) { }
}
