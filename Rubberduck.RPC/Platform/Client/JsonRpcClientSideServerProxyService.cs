using System;
using System.Threading.Tasks;
using StreamJsonRpc;
using System.IO.Pipes;

namespace Rubberduck.RPC.Platform.Client
{
    /// <summary>
    /// Represents a client-side server proxy service that can send notifications and request server responses.
    /// </summary>
    /// <typeparam name="TServerProxy">The type of the server proxy to communicate with.</typeparam>
    public class JsonRpcClientSideServerProxyService<TServerProxy> : NamedPipeJsonRpcClient
        where TServerProxy : class, IJsonRpcTarget
    {
        private static readonly JsonRpcProxyOptions _proxyOptions = new JsonRpcProxyOptions
        {
            EventNameTransform = JsonRpcNameTransforms.EventNameTransform,
            MethodNameTransform = JsonRpcNameTransforms.MethodNameTransform,
            ServerRequiresNamedArguments = true,
        };

        /// <summary>
        /// Creates a client-side server proxy service that can send notifications and request server responses.
        /// </summary>
        public JsonRpcClientSideServerProxyService(IRpcStreamFactory<NamedPipeClientStream> rpcStreamFactory) 
            : base(rpcStreamFactory)
        {
        }

        /// <summary>
        /// Sends a RPC message notifying the server of a client event.
        /// </summary>
        /// <param name="method">A delegate that invokes a method against a <c>TServerProxy</c>.</param>
        public async Task NotifyAsync(Func<TServerProxy, Task> method)
        {
            using (var stream = RpcStreamFactory.CreateNew())
            {
                await ConnectAsync(stream);
                var rpc = JsonRpc.Attach<TServerProxy>(stream);
                {
                    await method.Invoke(rpc);
                }
            }
        }

        /// <summary>
        /// Sends a RPC message notifying the server of a client event.
        /// </summary>
        /// <param name="method">A delegate that invokes a method against a <c>TServerProxy</c>.</param>
        /// <param name="parameter">A parameter object to send along with the notification.</param>
        public async Task NotifyAsync<TParameter>(Func<TServerProxy, TParameter, Task> method, TParameter parameter)
        {
            using (var stream = RpcStreamFactory.CreateNew())
            {
                await ConnectAsync(stream);
                var rpc = JsonRpc.Attach<TServerProxy>(stream);
                {
                    await method.Invoke(rpc, parameter);
                }
            }
        }


        /// <summary>
        /// Sends a RPC message requesting a response of a given data type from the server.
        /// </summary>
        /// <typeparam name="TResponse">The class type of the response object returned from the server.</typeparam>
        /// <param name="method">A delegate that invokes a method against any implementation of <c>IJsonRpcTarget</c>.</param>
        /// <returns>The server response.</returns>
        public async Task<TResponse> RequestAsync<TResponse>(Func<TServerProxy, Task<TResponse>> method)
            where TResponse : class, new()
        {
            using (var stream = RpcStreamFactory.CreateNew())
            {
                await ConnectAsync(stream);
                var rpc = JsonRpc.Attach<TServerProxy>(stream);
                
                return await method.Invoke(rpc);
            }
        }

        /// <summary>
        /// Sends a RPC message requesting a response of a given data type from the server.
        /// </summary>
        /// <typeparam name="TResponse">The class type of the response object returned from the server.</typeparam>
        /// <param name="method">A delegate that invokes a method against any implementation of <c>IJsonRpcTarget</c>.</param>
        /// <param name="parameter">A parameter object to send along with the request.</param>
        /// <returns>The server response.</returns>
        public async Task<TResponse> RequestAsync<TParameter, TResponse>(Func<TServerProxy, TParameter, Task<TResponse>> method, TParameter parameter)
            where TResponse : class, new()
        {
            using (var stream = RpcStreamFactory.CreateNew())
            {
                await ConnectAsync(stream);
                var rpc = JsonRpc.Attach<TServerProxy>(stream);
                
                return await method.Invoke(rpc, parameter);
            }
        }
    }
}