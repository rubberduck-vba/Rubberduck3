using System;

namespace Rubberduck.Editor
{
    public interface ILanguageServerConnectionStatusProvider
    {
        event EventHandler Connecting;
        event EventHandler Connected;
        event EventHandler Disconnected;
    }
}
