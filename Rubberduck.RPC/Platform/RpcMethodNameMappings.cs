using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rubberduck.RPC.Platform.Metadata;

namespace Rubberduck.RPC.Platform
{
    internal static class RpcMethodNameMappings
    {
        private static readonly HashSet<string> _rpcEndpointNames;

        private static readonly IDictionary<string, string> _mappedEventNames;
        private static IEnumerable<(string EventName, string RpcMethodName)> GetRpcEvents<TAttribute>(IEnumerable<EventInfo> events) where TAttribute : RubberduckSPAttribute
            => from e in events let attr = e.GetCustomAttribute<TAttribute>() where attr != null && !string.IsNullOrWhiteSpace(attr.MethodName) select (e.Name, attr.MethodName);

        private static readonly IDictionary<string, string> _mappedMethodNames;
        private static IEnumerable<(string MethodName, string RpcMethodName)> GetRpcMethods<TAttribute>(IEnumerable<MethodInfo> methods) where TAttribute : RubberduckSPAttribute
            => from m in methods let attr = m.GetCustomAttribute<TAttribute>() where attr != null && !string.IsNullOrWhiteSpace(attr.MethodName) select (m.Name, attr.MethodName);

        static RpcMethodNameMappings()
        {
            var types = typeof(IJsonRpcServer).Assembly.GetTypes().ToArray();

            // event names map to their LSP/RPC counterpart using [RubberduckSP] attributes:
            var events = types.SelectMany(t => t.GetEvents()).ToArray();
            var rpcEvents = GetRpcEvents<RubberduckSPAttribute>(events);

            _mappedEventNames = rpcEvents.ToDictionary(ev => ev.EventName, ev => ev.RpcMethodName);

            // method names map to their LSP/RPC counterpart using [LspCompliant].
            // methods with [JsonRpcMethod] are already mapped.
            var methods = types.SelectMany(t => t.GetMethods()).ToArray();
            var rpcMethods = GetRpcMethods<RubberduckSPAttribute>(methods);

            _mappedMethodNames = rpcMethods.Distinct().ToDictionary(m => m.MethodName, m => m.RpcMethodName);


            _rpcEndpointNames = _mappedEventNames.Values.Concat(_mappedMethodNames.Values).ToHashSet();
        }

        /// <summary>
        /// <c>true</c> if the specified <c>name</c> is a mapped event/notification, <c>false</c> otherwise.
        /// </summary>
        /// <remarks>
        /// Returns the mapped <c>rpcMethodName</c> if the lookup succeeds.
        /// </remarks>
        public static bool IsMappedEvent(string name, out string rpcMethodName)
        {
            if (_rpcEndpointNames.Contains(name))
            {
                rpcMethodName = name;
                return true;
            }

            return _mappedEventNames.TryGetValue(name, out rpcMethodName);
        }

        /// <summary>
        /// <c>true</c> if the specified <c>name</c> is a mapped method/request, <c>false</c> otherwise.
        /// </summary>
        /// <remarks>
        /// Returns the mapped <c>rpcMethodName</c> if the lookup succeeds.
        /// </remarks>
        public static bool IsMappedMethod(string name, out string rpcMethodName)
        {
            if (_rpcEndpointNames.Contains(name))
            {
                rpcMethodName = name;
                return true;
            }

            return _mappedMethodNames.TryGetValue(name, out rpcMethodName);
        }
    }
}
