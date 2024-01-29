using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using Rubberduck.InternalApi.Model.Declarations.Execution;
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
public interface IExecutableMemberScope : IExecutableSymbol
{
}

public abstract record class VBExecutableMember : VBTypeMember, IExecutableMemberScope
{
    public VBExecutableMember(Uri uri, string name, RubberduckSymbolKind kind, Accessibility accessibility, ProcedureSymbol declaration, TypedSymbol[]? definitions = null)
        : base(uri, name, kind, accessibility, declaration, definitions)
    {
    }

    public VBExecutableMember(Uri uri, string name, RubberduckSymbolKind kind, Accessibility accessibility, ProcedureSymbol? declaration = null, TypedSymbol[]? definitions = null, bool isUserDefined = false)
        : base(uri, name, kind, accessibility, declaration, definitions, isUserDefined)
    {
    }

    public bool? IsReachable { get; init; }

    public virtual ExecutionContext Execute(ExecutionContext context) => context;
}

public record class VBProcedureMember : VBExecutableMember
{
    public VBProcedureMember(Uri uri, string name, RubberduckSymbolKind kind, Accessibility accessibility, ProcedureSymbol declaration, TypedSymbol[]? definitions = null)
        : base(uri, name, kind, accessibility, declaration, definitions)
    {
    }

    public VBProcedureMember(Uri uri, string name, RubberduckSymbolKind kind, Accessibility accessibility, ProcedureSymbol? declaration = null, TypedSymbol[]? definitions = null, bool isUserDefined = false)
        : base(uri, name, kind, accessibility, declaration, definitions, isUserDefined)
    {
    }
}
