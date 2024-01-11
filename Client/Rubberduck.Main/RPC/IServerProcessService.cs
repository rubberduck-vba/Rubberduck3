using System;
using System.Diagnostics;

namespace Rubberduck.Main.RPC
{
    interface IServerProcessService : IDisposable
    {
        bool StartServerProcess();
        Process Process { get; }
    }
}