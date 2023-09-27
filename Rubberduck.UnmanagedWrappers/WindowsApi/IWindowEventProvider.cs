using System;

namespace Rubberduck.Unmanaged.WindowsApi
{
    public interface IWindowEventProvider : IDisposable
    {
        event EventHandler CaptionChanged;
        //event EventHandler<KeyPressEventArgs> KeyDown;
    }
}
