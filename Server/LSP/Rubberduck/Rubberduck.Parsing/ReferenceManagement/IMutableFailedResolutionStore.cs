using Rubberduck.Parsing.Model.Symbols;

namespace Rubberduck.Parsing.ReferenceManagement
{
    public interface IMutableFailedResolutionStore : IFailedResolutionStore
    {
        void AddUnresolvedMemberDeclaration(UnboundMemberDeclaration unresolvedMemberDeclaration);
        void AddUnboundDefaultMemberAccess(IdentifierReference unboundDefaultMemberAccess);
        void AddFailedLetCoercion(IdentifierReference failedLetCoercion);
        void AddFailedProcedureCoercion(IdentifierReference failedProcedureCoercion);
        void AddFailedIndexedDefaultMemberResolution(IdentifierReference failedIndexedDefaultMemberResolution);
    }
}
