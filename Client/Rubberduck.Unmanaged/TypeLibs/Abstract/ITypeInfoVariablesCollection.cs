using System.Collections.Generic;

namespace Rubberduck.Unmanaged.TypeLibs.Abstract
{
    public interface ITypeInfoVariablesCollection
    {
        int Count { get; }
        ITypeInfoVariable GetItemByIndex(int index);
        IEnumerator<ITypeInfoVariable> GetEnumerator();
    }
}