namespace Rubberduck.RPC.Platform.Model.Telemetry
{
    /// <summary>
    /// Defines the possible values of <c>TelemetryEvent.EventName</c>.
    /// </summary>
    public enum TelemetryEventName
    {
        /// <summary>
        /// A <c>RequestTelemetry</c> item.
        /// </summary>
        Request,
        /// <summary>
        /// A <c>DependencyTelemetry</c> item.
        /// </summary>
        Dependency,
        /// <summary>
        /// An <c>ExceptionTelemetry</c> item.
        /// </summary>
        Exception,
        /// <summary>
        /// A <c>TraceTelemetry</c> item.
        /// </summary>
        Trace,
        /// <summary>
        /// An <c>EventTelemetry</c> item.
        /// </summary>
        Event,
        /// <summary>
        /// A <c>MetricTelemetry</c> item.
        /// </summary>
        Metric,
        /// <summary>
        /// A <c>PageViewTelemetry</c> item.
        /// </summary>
        PageView,
    }

    /// <summary>
    /// Defines the possible values of <c>EventTelemetry.Name</c>.
    /// </summary>
    public enum EventTelemetryName
    {
        /* range 0-1000 not used */

        #region range 1000-1999 [VBE] add-in client host process telemetry
        /// <summary>
        /// The splash screen was displayed.
        /// </summary>
        VBE_OnSplashShown = 1000,

        /// <summary>
        /// An Office CommandBar Rubberduck menu command button was clicked in the native VBIDE main menu bar.
        /// </summary>
        VBE_OnRubberduckMainMenuBarMenuButtonClicked,
        /// <summary>
        /// An Office CommandBar Rubberduck menu command button was clicked in a native VBIDE context menu.
        /// </summary>
        VBE_OnRubberduckContextMenuButtonClicked,
        
        /// <summary>
        /// A VBProject was loaded in the VBIDE.
        /// </summary>
        VBE_OnProjectLoaded,
        /// <summary>
        /// A VBProject was unloaded in the VBIDE.
        /// </summary>
        VBE_OnProjectUnloaded,
        /// <summary>
        /// Project references were modified for the active project in the VBIDE.
        /// </summary>
        VBE_OnProjectReferencesModified,

        /// <summary>
        /// A VBComponent was added to the active project in the VBIDE.
        /// </summary>
        VBE_OnComponentAdded,
        /// <summary>
        /// A VBComponent was removed from the active project un the VBIDE.
        /// </summary>
        VBE_OnComponentRemoved,
        /// <summary>
        /// A VBComponent was imported into the active project in the VBIDE.
        /// </summary>
        VBE_OnComponentImported,

        /// <summary>
        /// A Rubberduck-initiated request to compile the active VBProject failed in the VBIDE.
        /// </summary>
        VBE_OnCompileRequestFailed,

        /// <summary>
        /// The <c>About</c> Rubberduck menu command was executed.
        /// </summary>
        VBE_OnAboutMenuCommandClicked,
        /// <summary>
        /// The <c>ShowRubberduckEditor</c> Rubberduck menu command was executed.
        /// </summary>
        VBE_OnShowRubberduckEditorMenuCommandClicked,

        // ...other menu commands would go here...
        #endregion

        #region range 2000-2999 [RDE] add-in client managed components telemetry

        /// <summary>
        /// The Rubberduck editor shell was displayed.
        /// </summary>
        RDE_OnEditorShellShown = 1000,
        /// <summary>
        /// The Rubberduck editor shell was closed.
        /// </summary>
        RDE_OnEditorShellClosed,

        /// <summary>
        /// A command was initiated using a hotkey in the Rubberduck Editor.
        /// </summary>
        RDE_OnHotkeyCommand,
        /// <summary>
        /// A menu button was clicked in the Rubberduck Editor.
        /// </summary>
        RDE_OnMenuButtonClicked,
        /// <summary>
        /// A menu button was clicked in a context menu of the Rubberduck Editor
        /// </summary>
        RDE_OnContextMenuButtonClicked,
        
        /// <summary>
        /// The <c>LoadWorkspace</c> command has executed.
        /// </summary>
        RDE_OnLoadWorkspaceCommand,
        /// <summary>
        /// The <c>Synchronize</c> command has executed.
        /// </summary>
        /// <remarks>
        /// Brings into the VBIDE modifications made in the RDE.
        /// </remarks>
        RDE_OnSynchronizeCommand,

        /// <summary>
        /// A quick-fix was selected from a code action drop-down in the Rubberduck Editor.
        /// </summary>
        /// <remarks>aka "ducky menu"</remarks>
        RDE_OnCodeActionQuickFixClicked,
        /// <summary>
        /// An 'ignore' resolution was selected from a code action drop-down in the Rubberduck editor.
        /// </summary>
        RDE_OnCodeActionIgnoreClicked,

        /// <summary>
        /// A refactoring action is being initiated in the Rubberduck editor.
        /// </summary>
        RDE_OnRefactoringActionCommand,
        /// <summary>
        /// A refactoring action was cancelled in the Rubberduck editor after a preview was shown.
        /// </summary>
        RDE_OnRefactoringPreviewCancelCommand,
        /// <summary>
        /// A refactoring action was confirmed in the Rubberduck editor after a preview was shown.
        /// </summary>
        RDE_OnRefacroringPreviewConfirmCommand,
        /// <summary>
        /// A code completion was confirmed by the user in the Rubberduck editor.
        /// </summary>
        RDE_OnConfirmCompletion,
        /// <summary>
        /// Code completion was actively dismissed (<c>ESC</c>) by the user in the Rubberduck editor.
        /// </summary>
        RDE_OnDismissCompletion,
        /// <summary>
        /// ParameterInfo was shown in the Rubberduck editor.
        /// </summary>
        RDE_OnParameterInfo,
        /// <summary>
        /// ParameterInfo was actively dismissed (<c>ESC</c>) by the user in the Rubberduck editor.
        /// </summary>
        RDE_OnDismissParameterInfo,

        #endregion

        #region range 3000-3999 [LSP] language server process telemetry

        /// <summary>
        /// The LSP client console GUI client main window was displayed.
        /// </summary>
        LSP_OnClientConsoleShown = 3000,
        /// <summary>
        /// The LSP client console GUI client main window was minimized.
        /// </summary>
        LSP_OnClientConsoleMinimized,
        /// <summary>
        /// The LSP client console GUI client main window was maximized.
        /// </summary>
        LSP_OnClientConsoleMaximized,
        /// <summary>
        /// The LSP client console GUI client main window was closed/hidden.
        /// </summary>
        LSP_OnClientConsoleHidden,
        /// <summary>
        /// The <c>RestartCommand</c> server command is being executed form the LSP console client.
        /// </summary>
        LSP_OnClientConsoleRestartCommand,
        /// <summary>
        /// The <c>ShutdownCommand</c> server command is being executed from the LSP console client.
        /// </summary>
        LSP_OnClientConsoleShutdownCommand,
        /// <summary>
        /// The <c>SaveAsCommand</c> client command is being executed in the LSP console client.
        /// </summary>
        LSP_OnClientConsoleSaveAsCommand,
        /// <summary>
        /// The <c>CopyCommand</c> client command is being executed in the LSP console client.
        /// </summary>
        LSP_OnClientConsoleCopyCommand,
        /// <summary>
        /// The <c>ClearCommand</c> client command is being executed in the LSP console client.
        /// </summary>
        LSP_OnClientConsoleClearCommand,
        /// <summary>
        /// The <c>SetTraceCommand</c> server command is being executed from the LSP console client.
        /// </summary>
        LSP_OnClientConsoleSetTraceCommand,
        /// <summary>
        /// A tab header was clicked in the LSP console client.
        /// </summary>
        LSP_OnClientConsoleTabHeaderClicked,
        /// <summary>
        /// LSP client/server settings were modified in the LSP console client.
        /// </summary>
        LSP_OnClientConsoleSaveSettings,
        /// <summary>
        /// Telemetry was opted-in in the LSP console client.
        /// </summary>
        LSP_OnTelemetryOptIn,
        /// <summary>
        /// Telemetry was opted-out (was previously opted-in) in the LSP console client.
        /// </summary>
        LSP_OnTelemetryOptOut,
        /// <summary>
        /// The <c>TelemetryDeleteCommand</c> server command is being executed from the LSP console client.
        /// </summary>
        LSP_OnTelemetryDeleteCommand,
        /// <summary>
        /// The <c>TelemetryExportCommand</c> server command is being executed from the LSP console client.
        /// </summary>
        LSP_OnTelemetryExportCommand,
        /// <summary>
        /// The <c>TelemetryUploadCommand</c> server command is being executed from the LSP console client.
        /// </summary>
        LSP_OnTelemetryUploadCommand,

        /// <summary>
        /// A code action command is executing.
        /// </summary>
        LSP_OnCodeActionCommand,

        #endregion

        #region range 4000-4999 [SQL] database server process telemetry

        /// <summary>
        /// The LocalDb client console GUI client main window was displayed.
        /// </summary>
        SQL_OnClientConsoleShown = 4000,
        /// <summary>
        /// The LocalDb client console GUI client main window was minimized.
        /// </summary>
        SQL_OnClientConsoleMinimized,
        /// <summary>
        /// The LocalDb client console GUI client main window was maximized.
        /// </summary>
        SQL_OnClientConsoleMaximized,
        /// <summary>
        /// The LocalDb client console GUI client main window was closed/hidden.
        /// </summary>
        SQL_OnClientConsoleHidden,
        /// <summary>
        /// The <c>RestartCommand</c> server command is being executed form the LocalDb console client.
        /// </summary>
        SQL_OnClientConsoleRestartCommand,
        /// <summary>
        /// The <c>ShutdownCommand</c> server command is being executed from the LocalDb console client.
        /// </summary>
        SQL_OnClientConsoleShutdownCommand,
        /// <summary>
        /// The <c>SaveAsCommand</c> client command is being executed in the LocalDb console client.
        /// </summary>
        SQL_OnClientConsoleSaveAsCommand,
        /// <summary>
        /// The <c>CopyCommand</c> client command is being executed in the LocalDb console client.
        /// </summary>
        SQL_OnClientConsoleCopyCommand,
        /// <summary>
        /// The <c>ClearCommand</c> client command is being executed in the LocalDb console client.
        /// </summary>
        SQL_OnClientConsoleClearCommand,
        /// <summary>
        /// The <c>SetTraceCommand</c> server command is being executed from the LocalDb console client.
        /// </summary>
        SQL_OnClientConsoleSetTraceCommand,
        /// <summary>
        /// The <c>Console</c> tab header was clicked in the LocalDb console client.
        /// </summary>
        SQL_OnClientConsoleTabHeaderClicked,
        /// <summary>
        /// The <c>Telemetry</c> tab header was clicked in the LocalDb console client.
        /// </summary>
        SQL_OnClientTelemetryTabHeaderClicked,
        /// <summary>
        /// The <c>Settings</c> tab header was clicked in the LocalDb console client.
        /// </summary>
        SQL_OnClientSettingsTabHeaderClicked,
        /// <summary>
        /// SQL client/server settings were modified in the LocalDb console client.
        /// </summary>
        SQL_OnClientConsoleSaveSettings,

        #endregion
    }
}
