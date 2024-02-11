using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Abstract;

public interface IRubberduckParserErrorListenerFactory
{
    IRubberduckParseErrorListener Create(WorkspaceFileUri uri, CodeKind codeKind);
}
