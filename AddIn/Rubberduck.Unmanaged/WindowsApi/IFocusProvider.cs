using Rubberduck.Unmanaged.Events;
using System;

namespace Rubberduck.Unmanaged.WindowsApi
{
    public interface IFocusProvider : IDisposable
    {
        event EventHandler<WindowChangedEventArgs> FocusChange;
    }
}
