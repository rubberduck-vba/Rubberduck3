using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.UpdateServer
{
    interface IVersionCheckService
    {
        object/*TagInfo*/ GetLatestTagInfo();
    }

    interface IUpdateService
    {
        /// <summary>
        /// Downloads the installer package for the specified tag, then commands the client to shutdown and disconnect from the host. 
        /// Once the client has been unloaded, all Rubberduck libraries (except UpdateServer) are no longer in use and should be overwritable.
        /// After updating to the latest version, the service attemps to reload the updated Rubberduck client, which starts the updated server.
        /// </summary>
        void Update(/*TagInfo info*/);
    }
}
