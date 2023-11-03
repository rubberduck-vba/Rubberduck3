using Rubberduck.Unmanaged.TypeLibs.Abstract;
using Rubberduck.Unmanaged.TypeLibs.Unmanaged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Rubberduck.Unmanaged
{
    public sealed class ComWrapperEnumerator<TWrapperItem> : IEnumerator<TWrapperItem>
        where TWrapperItem : class
    {
        private readonly Func<object, TWrapperItem> _itemWrapper;
        private readonly IEnumVARIANT _enumeratorRCW;
        private TWrapperItem _currentItem;

        public ComWrapperEnumerator(object source, Func<object, TWrapperItem> itemWrapper)
        {
            _itemWrapper = itemWrapper;

            if (source != null)
            {
                _enumeratorRCW = (IEnumVARIANT)IDispatchHelper.PropertyGet_NoArgs((IDispatch)source, (int)IDispatchHelper.StandardDispIds.DISPID_ENUM);
                ((IEnumerator)this).Reset();  // precaution 
            }
        }

        void IEnumerator.Reset()
        {
            if (!IsWrappingNullReference)
            {
                int hr = _enumeratorRCW.Reset();
                if (hr < 0)
                {
                    throw Marshal.GetExceptionForHR(hr);
                }
            }
        }

        public bool IsWrappingNullReference => _enumeratorRCW == null;

        public TWrapperItem Current => _currentItem;
        object IEnumerator.Current => _currentItem;

        bool IEnumerator.MoveNext()
        {
            if (IsWrappingNullReference)
            {
                return false;
            }

            _currentItem = null;

            int hr = _enumeratorRCW.Next(1, out object currentItemRCW, out uint celtFetched);
            // hr == S_FALSE (1) or S_OK (0), or <0 means error

            _currentItem = _itemWrapper.Invoke(currentItemRCW);     // creates a null wrapped reference even on end/error, just as a precaution

            if (hr < 0)
            {
                throw Marshal.GetExceptionForHR(hr);
            }

            return celtFetched == 1;      // celtFetched will be 0 when we reach the end of the collection
        }

        public void Dispose()
        {
            if (!IsWrappingNullReference)
            {
                Marshal.ReleaseComObject(_enumeratorRCW);
            }
        }
    }
}