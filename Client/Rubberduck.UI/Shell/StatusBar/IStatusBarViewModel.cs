using Rubberduck.InternalApi.Model;
using Rubberduck.UI.Command;
using System.Collections;
using System.Collections.Generic;

namespace Rubberduck.UI.Shell.StatusBar
{
    /// <summary>
    /// The base interface for a status bar view model.
    /// </summary>
    public interface IStatusBarViewModel
    {
        /// <summary>
        /// Gets/sets the current server state.
        /// </summary>
        string StatusText { get; set; }
    }

    /// <summary>
    /// The base interface for a status bar view model that can display progress updates.
    /// </summary>
    public interface IProgressStatusViewModel : IStatusBarViewModel
    {
        /// <summary>
        /// Gets or sets the current progress indicator value.
        /// </summary>
        int ProgressValue { get; set; }
        /// <summary>
        /// Gets or sets the progress indicator max value.
        /// </summary>
        int ProgressMaxValue { get; set; }
    }

    /// <summary>
    /// A status bar view model that shows language server connection status.
    /// </summary>
    public interface ILanguageServerStatusViewModel : IStatusBarViewModel
    {
        /// <summary>
        /// Gets or sets the server connection state.
        /// </summary>
        bool IsConnected { get; set; }
    }

    public interface INotificationViewModel
    {
        string Title { get; set; }
        string Description { get; set; }
        MessageAction[] Actions { get; } 
    }

    /// <summary>
    /// A status bar view model that shows notifications.
    /// </summary>
    public interface INotificationStatusViewModel : IStatusBarViewModel
    {
        ICollection<INotificationViewModel> Notifications { get; }
    }

    /// <summary>
    /// An interface representing the shell window status bar.
    /// </summary>
    public interface IShellStatusBarViewModel : ILanguageServerStatusViewModel, IProgressStatusViewModel, INotificationStatusViewModel
    {

    }

    /// <summary>
    /// An interface representing a document window status bar.
    /// </summary>
    public interface IDocumentStatusViewModel : IProgressStatusViewModel
    {
        /// <summary>
        /// The number of lines in the document.
        /// </summary>
        int DocumentLines { get; set; }
        /// <summary>
        /// The number of characters in the document.
        /// </summary>
        int DocumentLength { get; set; }
        /// <summary>
        /// The current caret position in the document.
        /// </summary>
        int CaretOffset { get; set; }
        /// <summary>
        /// The current line position of the selection caret.
        /// </summary>
        int CaretLine { get; set; }
        /// <summary>
        /// The current column position of the selection caret.
        /// </summary>
        int CaretColumn { get; set; }
    }

    public interface IDiagnosticViewModel
    {
        string Title { get; set; }
        string Description { get; set; }
        int Severity { get; set; } // TODO import InspectionSeverity enum
        DocumentOffset DocumentOffset { get; set; }
    }

    public interface ISourceFileStatusViewModel : IDocumentStatusViewModel
    {
        IDiagnosticViewModel[] Diagnostics { get; set; }
    }
}
