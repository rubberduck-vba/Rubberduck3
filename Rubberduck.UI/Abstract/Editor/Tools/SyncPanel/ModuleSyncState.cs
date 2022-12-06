using System;

namespace Rubberduck.UI.Abstract
{
    [Flags]
    public enum ModuleSyncState
    {
        /// <summary>
        /// The module is not loaded into the editor shell as a document tab.
        /// </summary>
        NotLoaded = 0,
        /// <summary>
        /// Module failed to load.
        /// </summary>
        LoadError = 1,
        /// <summary>
        /// The document is loaded and synchronized with the corresponding VBE module.
        /// </summary>
        OK = 2,
        /// <summary>
        /// The document was modified in the editor document tab.
        /// </summary>
        ModifiedRDE = 4,
        /// <summary>
        /// The corresponding VBE module was modified.
        /// </summary>
        ModifiedVBE = 8,
        /// <summary>
        /// The document was removed from its parent project in the editor shell.
        /// </summary>
        DeletedRDE = 16,
        /// <summary>
        /// The corresponding VBE module was removed from its parent project in the VBE.
        /// </summary>
        DeletedVBE = 32,
    }
}
