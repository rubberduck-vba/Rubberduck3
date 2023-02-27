using Rubberduck.Parsing.Model.Symbols;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Rubberduck.Parsing.ReferenceManagement
{
    public class ConcurrentFailedResolutionStore : IMutableFailedResolutionStore
    {
        private readonly ConcurrentBag<UnboundMemberDeclaration> _unresolvedMemberDeclarations = new ConcurrentBag<UnboundMemberDeclaration>();
        private readonly ConcurrentBag<IdentifierReference> _unboundDefaultMemberAccesses = new ConcurrentBag<IdentifierReference>();
        private readonly ConcurrentBag<IdentifierReference> _failedLetCoercions = new ConcurrentBag<IdentifierReference>();
        private readonly ConcurrentBag<IdentifierReference> _failedProcedureCoercions = new ConcurrentBag<IdentifierReference>();
        private readonly ConcurrentBag<IdentifierReference> _failedIndexedDefaultMemberResolutions = new ConcurrentBag<IdentifierReference>();

        public IReadOnlyCollection<UnboundMemberDeclaration> UnresolvedMemberDeclarations => _unresolvedMemberDeclarations;
        public IReadOnlyCollection<IdentifierReference> UnboundDefaultMemberAccesses => _unboundDefaultMemberAccesses;
        public IReadOnlyCollection<IdentifierReference> FailedLetCoercions => _failedLetCoercions;
        public IReadOnlyCollection<IdentifierReference> FailedProcedureCoercions => _failedProcedureCoercions;
        public IReadOnlyCollection<IdentifierReference> FailedIndexedDefaultMemberResolutions => _failedIndexedDefaultMemberResolutions;

        public void AddUnresolvedMemberDeclaration(UnboundMemberDeclaration unresolvedMemberDeclaration)
        {
            _unresolvedMemberDeclarations.Add(unresolvedMemberDeclaration);
        }

        public void AddUnboundDefaultMemberAccess(IdentifierReference unboundDefaultMemberAccess)
        {
            _unboundDefaultMemberAccesses.Add(unboundDefaultMemberAccess);
        }

        public void AddFailedLetCoercion(IdentifierReference failedLetCoercion)
        {
            _failedLetCoercions.Add(failedLetCoercion);
        }

        public void AddFailedProcedureCoercion(IdentifierReference failedProcedureCoercion)
        {
            _failedProcedureCoercions.Add(failedProcedureCoercion);
        }

        public void AddFailedIndexedDefaultMemberResolution(IdentifierReference failedIndexedDefaultMemberResolution)
        {
            _failedIndexedDefaultMemberResolutions.Add(failedIndexedDefaultMemberResolution);
        }
    }
}
