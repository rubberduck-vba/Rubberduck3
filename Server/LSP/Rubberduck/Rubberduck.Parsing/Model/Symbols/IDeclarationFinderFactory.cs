using System.Collections.Generic;
using Rubberduck.Parsing.Annotations;
using Rubberduck.Parsing.DeclarationCaching;
using Rubberduck.Parsing.ReferenceManagement;
using Rubberduck.VBEditor;
using Rubberduck.VBEditor.SafeComWrappers.Abstract;

namespace Rubberduck.Parsing.Model.Symbols
{
    public interface IDeclarationFinderFactory
    {
        IDeclarationFinder Create(IReadOnlyList<Declaration> declarations,
            IEnumerable<IParseTreeAnnotation> annotations,
            IReadOnlyDictionary<QualifiedModuleName, LogicalLineStore> logicalLines,
            IReadOnlyDictionary<QualifiedModuleName, IFailedResolutionStore> failedResolutionStores,
            IHostApplication hostApp);
        void Release(IDeclarationFinder declarationFinder);
    }
}
