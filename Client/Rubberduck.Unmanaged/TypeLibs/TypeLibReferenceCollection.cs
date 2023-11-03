using Rubberduck.Unmanaged.TypeLibs.Abstract;
using Rubberduck.Unmanaged.TypeLibs.Utility;

namespace Rubberduck.Unmanaged.TypeLibs
{
    /// <summary>
    /// Exposes an enumerable collection of references used by the VBE type library
    /// </summary>
    internal class TypeLibReferenceCollection : IndexedCollectionBase<ITypeLibReference>, ITypeLibReferenceCollection
    {
        private readonly ITypeLibVBEExtensions _parent;
        public TypeLibReferenceCollection(ITypeLibVBEExtensions parent) => _parent = parent;
        public override int Count => _parent.GetVBEReferencesCount();
        public override ITypeLibReference GetItemByIndex(int index) => _parent.GetVBEReferenceByIndex(index);
    }
}
