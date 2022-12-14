using System.Collections.Generic;
using Rubberduck.Parsing.Model.Symbols;
using Rubberduck.VBEditor;

namespace Rubberduck.Parsing.COM.Abstract
{
    public interface IReferencedDeclarationsCollector
    {
        IReadOnlyCollection<Declaration> CollectedDeclarations(ReferenceInfo reference);
    }
}
