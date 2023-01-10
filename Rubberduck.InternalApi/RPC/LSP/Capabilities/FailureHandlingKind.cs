namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class FailureHandlingKind
    {
        /// <summary>
        /// Applying the workspace change is simply aborted if one of the changes provided fails. All operations executed before the failing operation stay executed.
        /// </summary>
        public const string Abort = "abort";
        /// <summary>
        /// All operations are executed transactionally. That means they either all succeed, or no changes at all are applied to the workspace.
        /// </summary>
        public const string Transactional = "transactional";
        /// <summary>
        /// The client tries to undo the operations already executed. Note: there is no guarantee that this is succeeding.
        /// </summary>
        public const string Undo = "undo";
        /// <summary>
        /// If the workspace edit contains only textual file changes they are executed transactionally. If resource changes are part of the change, the failure handling strategy is 'abort'.
        /// </summary>
        public const string TextOnlyTransactional = "textOnlyTransactional";
    }
}
