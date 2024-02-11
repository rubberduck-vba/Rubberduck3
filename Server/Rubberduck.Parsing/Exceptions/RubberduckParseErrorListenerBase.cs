using Antlr4.Runtime;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Exceptions;

public class RubberduckParseErrorListenerBase : BaseErrorListener, IRubberduckParseErrorListener
{
    public RubberduckParseErrorListenerBase(WorkspaceFileUri uri, CodeKind codeKind)
    {
        Uri = uri;
        CodeKind = codeKind;
    }

    protected WorkspaceFileUri Uri { get; }
    protected CodeKind CodeKind { get; }
 }
