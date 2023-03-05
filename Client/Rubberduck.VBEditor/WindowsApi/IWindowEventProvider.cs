using System;
using Rubberduck.VBEditor.Events;
using KeyPressEventArgs = Rubberduck.VBEditor.Events.KeyPressEventArgs;

namespace Rubberduck.VBEditor.WindowsApi
{
    public interface IWindowEventProvider : IDisposable
    {
        event EventHandler CaptionChanged;
        event EventHandler<KeyPressEventArgs> KeyDown;
    }
}
