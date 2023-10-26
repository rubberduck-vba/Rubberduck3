using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Extensibility;
using Rubberduck.Unmanaged.Registration;
using Rubberduck.Main.Root;

namespace Rubberduck.Main
{
    /// <remarks>
    /// Special thanks to Carlos Quintero (MZ-Tools) for providing the general structure here.
    /// </remarks>
    [
        ComVisible(true),
        Guid(RubberduckGuid.ExtensionGuid),
        ProgId(RubberduckProgId.ExtensionProgId),
        ClassInterface(ClassInterfaceType.None),
        ComDefaultInterface(typeof(IDTExtensibility2)),
        EditorBrowsable(EditorBrowsableState.Never)
    ]
    public class Extension : IDTExtensibility2
    {
        private ILogger? _logger = null; // TODO
        private IVBIDEAddIn? _rubberduck;

        public void OnConnection(object Application, ext_ConnectMode ConnectMode, object AddInInst, ref Array custom)
        {
            try
            {
                _rubberduck = new RubberduckAddIn(this,
                    RootComWrapperFactory.GetVbeWrapper(Application),
                    RootComWrapperFactory.GetAddInWrapper(AddInInst));

                switch (ConnectMode)
                {
                    case ext_ConnectMode.ext_cm_Startup:
                        // normal execution path - don't initialize just yet, wait for OnStartupComplete to be called by the host.
                        break;
                    case ext_ConnectMode.ext_cm_AfterStartup:
                        _rubberduck.Initialize();
                        break;
                }
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "{message}", e.Message);
            }
        }

        public void OnStartupComplete(ref Array custom)
        {
            _rubberduck?.Initialize();
        }

        public void OnBeginShutdown(ref Array custom)
        {
            _rubberduck?.Shutdown();
        }

        public void OnDisconnection(ext_DisconnectMode RemoveMode, ref Array custom)
        {
            _rubberduck?.Shutdown();
            _rubberduck = null;
        }

        public void OnAddInsUpdate(ref Array custom) { /* no-op / unhandled */ }
    }
}