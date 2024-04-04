using System.Collections.Generic;

namespace Rubberduck.Unmanaged.TypeLibs.Abstract
{
    public interface ITypeInfoWrapperCollection
    {
        int Count { get; }
        ITypeInfoWrapper GetItemByIndex(int index);
        ITypeInfoWrapper Find(string searchTypeName);
        ITypeInfoWrapper Get(string searchTypeName);
        IEnumerator<ITypeInfoWrapper> GetEnumerator();
    }
}