﻿using Rubberduck.Unmanaged.TypeLibs.Abstract;
using Rubberduck.Unmanaged.TypeLibs.Utility;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace Rubberduck.Unmanaged.TypeLibs
{
    /// <summary>
    /// Exposes an enumerable collection of variables[fields] provided by the
    /// <see cref="ComTypes.ITypeInfo"/>
    /// </summary>
    internal class TypeInfoVariablesCollection : IndexedCollectionBase<ITypeInfoVariable>, ITypeInfoVariablesCollection
    {
        protected readonly ComTypes.ITypeInfo Parent;
        protected readonly int _count;
        
        public TypeInfoVariablesCollection(ComTypes.ITypeInfo parent, ComTypes.TYPEATTR attributes)
        {
            Parent = parent;
            _count = attributes.cVars;
        }

        public override int Count => _count;
        
        public override ITypeInfoVariable GetItemByIndex(int index) => new TypeInfoVariable(Parent, index);
    }
}
