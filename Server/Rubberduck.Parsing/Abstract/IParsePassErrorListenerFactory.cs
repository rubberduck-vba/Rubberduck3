using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Abstract;

public interface IParsePassErrorListenerFactory
{
    IRubberduckParseErrorListener Create(WorkspaceFileUri uri, CodeKind codeKind);
}
