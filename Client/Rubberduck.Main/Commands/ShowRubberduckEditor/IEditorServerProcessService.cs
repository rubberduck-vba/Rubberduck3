using System;
using System.Diagnostics;

namespace Rubberduck.Main.Commands.ShowRubberduckEditor
{
    public interface IEditorServerProcessService
    {
        /// <summary>
        /// Starts the editor process, or brings it to front if it's already running.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the process was started; <c>false</c> if it is already running.
        /// </returns>
        bool StartEditorProcess();
        /// <summary>
        /// The editor process.
        /// </summary>
        /// <remarks>
        /// Throws if <c>StartEditorProcess</c> was not called first.
        /// </remarks>
        /// <exception cref="NullReferenceException" />
        Process Process { get; }
    }
}