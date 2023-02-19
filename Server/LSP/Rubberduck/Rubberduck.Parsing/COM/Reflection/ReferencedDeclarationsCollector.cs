using System.Collections.Generic;
using Rubberduck.Parsing.COM.Abstract;
using Rubberduck.Parsing.Model.ComReflection;
using Rubberduck.Parsing.Model.Symbols;
using Rubberduck.VBEditor;

namespace Rubberduck.Parsing.COM.Reflection
{
    public abstract class ReferencedDeclarationsCollectorBase : IReferencedDeclarationsCollector
    {
        private readonly IDeclarationsFromComProjectLoader _declarationsFromComProjectLoader;

        protected ReferencedDeclarationsCollectorBase(IDeclarationsFromComProjectLoader declarationsFromComProjectLoader)
        {
            _declarationsFromComProjectLoader = declarationsFromComProjectLoader;
        }


        public abstract IReadOnlyCollection<Declaration> ExtractDeclarations(ReferenceInfo reference);


        protected IReadOnlyCollection<Declaration> LoadDeclarationsFromComProject(ComProject type, string projectId = null)
        {
            return _declarationsFromComProjectLoader.LoadDeclarations(type, projectId);
        } 
    }
}
