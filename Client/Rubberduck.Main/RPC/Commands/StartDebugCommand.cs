using Rubberduck.InternalApi.ServerPlatform;
using System;

namespace Rubberduck.Main.RPC.Commands
{
    /// <summary>
    /// A command that is sent from the editor to the add-in, to compile and run the project.
    /// Parameter is expected to be a string that identifies the project and (parameterless) entry point to run, e.g. "Project1.Module1.DoSomething"
    /// </summary>
    public class StartDebugCommand : RpcCommandBase
    {
        public override void Execute(object? param)
        {
            throw new NotImplementedException();
        }
    }
}
