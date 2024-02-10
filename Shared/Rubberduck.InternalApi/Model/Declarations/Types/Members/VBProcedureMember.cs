using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

/// <summary>
/// Describes a <c>VBTypeMember</c> that can be executed with an execution context.
/// </summary>
public abstract record class VBExecutableMember : VBTypeMember
{
    public VBExecutableMember(Uri uri, string name, RubberduckSymbolKind kind, Accessibility accessibility, Symbol? declaration, TypedSymbol[]? definitions = null, bool isUserDefined = false)
        : base(uri, name, kind, accessibility, declaration, definitions, isUserDefined)
    {
    }

    public bool? IsReachable { get; init; }

    public virtual VBTypedValue? Evaluate(ref VBExecutionScope context) => ((IExecutable)Declaration!).Evaluate(ref context);
    public virtual VBTypedValue? Execute(ref VBExecutionContext context) => ((IExecutable)Declaration!).Execute(ref context);
}

public record class VBProcedureMember : VBExecutableMember
{
    public VBProcedureMember(Uri uri, string name, RubberduckSymbolKind kind, Accessibility accessibility, Symbol? declaration, TypedSymbol[]? definitions = null, bool isUserDefined = false)
        : base(uri, name, kind, accessibility, declaration, definitions, isUserDefined)
    {
    }
}
