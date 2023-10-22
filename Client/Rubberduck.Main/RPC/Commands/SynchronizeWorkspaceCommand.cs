using Rubberduck.InternalApi.ServerPlatform;
using System;

namespace Rubberduck.Main.RPC.Commands
{
    /// <summary>
    /// A command that is sent from the editor to the add-in, to import the workspace project files back into the VBE.
    /// </summary>
    public class SynchronizeWorkspaceCommand : RpcCommandBase
    {
        public override void Execute(object? param)
        {
            throw new NotImplementedException();
        }
    }
}
