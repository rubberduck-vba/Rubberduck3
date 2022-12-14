using Rubberduck.Parsing.Model.Symbols;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.Parsing.ReferenceManagement
{
    public class FailedResolutionStore : IFailedResolutionStore
    {
        public IReadOnlyCollection<UnboundMemberDeclaration> UnresolvedMemberDeclarations { get; }
        public IReadOnlyCollection<IdentifierReference> UnboundDefaultMemberAccesses { get; }
        public IReadOnlyCollection<IdentifierReference> FailedLetCoercions { get; }
        public IReadOnlyCollection<IdentifierReference> FailedProcedureCoercions { get; }
        public IReadOnlyCollection<IdentifierReference> FailedIndexedDefaultMemberResolutions { get; }

        public FailedResolutionStore(
            IReadOnlyCollection<UnboundMemberDeclaration> unresolvedMemberDeclarations,
            IReadOnlyCollection<IdentifierReference> unboundDefaultMemberAccesses,
            IReadOnlyCollection<IdentifierReference> failedLetCoercions,
            IReadOnlyCollection<IdentifierReference> failedProcedureCoercions,
            IReadOnlyCollection<IdentifierReference> failedIndexedDefaultMemberResolutions)
        {
            UnresolvedMemberDeclarations = unresolvedMemberDeclarations;
            UnboundDefaultMemberAccesses = unboundDefaultMemberAccesses;
            FailedLetCoercions = failedLetCoercions;
            FailedProcedureCoercions = failedProcedureCoercions;
            FailedIndexedDefaultMemberResolutions = failedIndexedDefaultMemberResolutions;
        }

        public FailedResolutionStore(IMutableFailedResolutionStore mutableStore)
            : this(
                mutableStore.UnresolvedMemberDeclarations.ToHashSet(),
                mutableStore.UnboundDefaultMemberAccesses.ToHashSet(),
                mutableStore.FailedLetCoercions.ToHashSet(),
                mutableStore.FailedProcedureCoercions.ToHashSet(),
                mutableStore.FailedIndexedDefaultMemberResolutions.ToHashSet())
        { }

        public FailedResolutionStore()
        : this(
            new List<UnboundMemberDeclaration>(),
            new List<IdentifierReference>(),
            new List<IdentifierReference>(),
            new List<IdentifierReference>(),
            new List<IdentifierReference>())
        { }
    }
}
