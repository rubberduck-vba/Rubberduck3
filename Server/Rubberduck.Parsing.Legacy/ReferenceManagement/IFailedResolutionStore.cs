using Rubberduck.Parsing.Model.Symbols;
using System.Collections.Generic;

namespace Rubberduck.Parsing.ReferenceManagement
{
    public interface IFailedResolutionStore
    {
        IReadOnlyCollection<UnboundMemberDeclaration> UnresolvedMemberDeclarations { get; }
        IReadOnlyCollection<IdentifierReference> UnboundDefaultMemberAccesses { get; }
        IReadOnlyCollection<IdentifierReference> FailedLetCoercions { get; }
        IReadOnlyCollection<IdentifierReference> FailedProcedureCoercions { get; }
        IReadOnlyCollection<IdentifierReference> FailedIndexedDefaultMemberResolutions { get; }
    }
}
