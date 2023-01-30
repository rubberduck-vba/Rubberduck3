using StreamJsonRpc;
using System;

namespace Rubberduck.RPC.Platform.Metadata
{
    /// <summary>
    /// An attribute that marks its target as defined in the Language Server Protocol (LSP)
    /// and intended to be compliant with the protocol specification.
    /// </summary>
    /// <remarks>
    /// Support is for protocol version 3.17 and above, unless specified otherwise.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Interface, Inherited = true)]
    public class LspCompliantAttribute : RubberduckSPAttribute 
    {
        /// <summary>
        /// The original LSP version implementation, released in <c>MinRubberduckVersion</c>.
        /// </summary>
        /// <see cref="RubberduckSPAttribute.MinRubberduckVersion"/>
        public const string MinProtocolVersion = "3.17";

        /// <summary>
        /// Marks a member as defined by the <em>Language Server Protocol (LSP)</em> and intended to be compliant with the protocol specification.
        /// </summary>
        /// <param name="name">The LSP method name for this attribute's target.</param>
        /// <param name="lspVersion">The minimum required supported LSP version for this attribute's target.</param>
        public LspCompliantAttribute(string name = null, string lspVersion = MinProtocolVersion, string rdVersion = MinRubberduckVersion) 
            : base(name, lspVersion, rdVersion)
        {
        }

        public override string Protocol { get; } = "LSP";
    }
}
