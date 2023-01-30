using System;

namespace Rubberduck.RPC.Platform.Metadata
{
    /// <summary>
    /// Marks a member as defined by the <em>Rubberduck Server Protocol (RSP)</em>, meaning it's about a custom RSP request or notification.
    /// </summary>
    /// <remarks>
    /// Used for communications between Server.LocalDb and Server.LSP, and/or Server.LSP and Server.Telemetry processes.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Interface, Inherited = true)]
    public class RubberduckSPAttribute : Attribute
    {
        /// <summary>
        /// The minimum Rubberduck release version to suppport <em>any</em> of this.
        /// </summary>
        public const string MinRubberduckVersion = "3.0";
        /// <summary>
        /// The version number attributed to <c>RSP</c> at the release of <c>MinRubberduckVersion</c>.
        /// </summary>
        private const string MinProtocolVersion = "1.0";

        /// <summary>
        /// Marks a member as defined by the <em>Rubberduck Server Protocol (RSP)</em>, meaning it's about a custom RSP request or notification.
        /// </summary>
        /// <param name="methodName">The RSP method name/path.</param>
        /// <param name="version">The minimum required Rubberduck release version that supports this method/path.</param>
        public RubberduckSPAttribute(string methodName = null, string rdVersion = MinRubberduckVersion, string protocolVersion = MinProtocolVersion)
        {
            MethodName = methodName;
            RubberduckVersion = rdVersion;
            ProtocolVersion = protocolVersion;
        }

        /// <summary>
        /// Identifies the protocol this member is defined in.
        /// </summary>
        public virtual string Protocol { get; } = "RSP";

        /// <summary>
        /// The RPC method name to use for this member.
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// The minimum required Rubberduck version that supports this attribute's target.
        /// </summary>
        /// <remarks>
        /// Refers to an unreleased semantic version number at the moment of the build; '<c>3.0</c>' unless specified otherwise.
        /// </remarks>
        public string RubberduckVersion { get; }

        /// <summary>
        /// The minimum required supported LSP version for this attribute's target.
        /// </summary>
        /// <remarks>The latest supported LSP version that defines this target at the moment of the build; '<c>3.17</c>' unless specified otherwise.</remarks>
        public string ProtocolVersion { get; }
    }
}
