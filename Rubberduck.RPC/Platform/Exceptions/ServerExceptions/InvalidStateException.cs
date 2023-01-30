using Rubberduck.RPC.Proxy.SharedServices;
using System.Collections.Generic;

namespace Rubberduck.RPC.Platform.Exceptions
{

    /// <summary>
    /// An exception that is thrown when attempting to execute a feature without the protocol-mandated server state for its command.
    /// </summary>
    public class InvalidStateException : ServerException
    {
        /// <summary>
        /// An exception that is thrown when attempting to execute a feature without the protocol-mandated server state for its command.
        /// </summary>
        /// <param name="source">Describes the origin of the exception.</param>
        /// <param name="actual">The current state of the server.</param>
        /// <param name="expected">The expected server state(s).</param>
        public InvalidStateException(string source, ServerStatus actual, params ServerStatus[] expected)
            : base(source, $"Command cannot be executed in current server state.")
        {
            Source = source;
            Expected = expected;
            Actual = actual;
        }


        /// <summary>
        /// The expected current server state(s).
        /// </summary>
        public IReadOnlyCollection<ServerStatus> Expected { get; }

        /// <summary>
        /// The actual current server state.
        /// </summary>
        public ServerStatus Actual { get; }

        public override string Verbose => $"Expected: ['{string.Join("','", Expected)}'], but was: '{Actual}'.";
    }
}
