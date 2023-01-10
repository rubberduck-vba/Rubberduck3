namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    /// <summary>
    /// Extra annotations that tweak the rendering of a completion item.
    /// </summary>
    public class CompletionItemTag
    {
        /// <summary>
        /// Render a completion as obsolete, usually using a strike-out.
        /// </summary>
        public const int Deprecated = 1;
    }
}
