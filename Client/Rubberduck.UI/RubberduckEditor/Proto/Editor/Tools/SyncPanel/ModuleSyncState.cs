using System;

namespace Rubberduck.UI.RubberduckEditor.Proto.Editor.Tools.SyncPanel
{
    [Flags]
    public enum ModuleSyncState
    {
        /// <summary>
        /// The module is not loaded into the editor shell as a document tab.
        /// </summary>
        NotLoaded = 0,
        /// <summary>
        /// Module failed to load (VBE -> RDE).
        /// </summary>
        LoadError = 1,
        /// <summary>
        /// Module failed to sync (RDE -> VBE).
        /// </summary>
        SyncError = 2,
        /// <summary>
        /// The document is loaded and synchronized with the corresponding VBE module.
        /// </summary>
        OK = 4,
        /// <summary>
        /// The document was modified in the editor document tab.
        /// </summary>
        ModifiedRDE = 8,
        /// <summary>
        /// The corresponding VBE module was modified.
        /// </summary>
        ModifiedVBE = 16,
        /// <summary>
        /// The document was removed from its parent project in the editor shell.
        /// </summary>
        DeletedRDE = 32,
        /// <summary>
        /// The corresponding VBE module was removed from its parent project in the VBE.
        /// </summary>
        DeletedVBE = 64,
    }
}
