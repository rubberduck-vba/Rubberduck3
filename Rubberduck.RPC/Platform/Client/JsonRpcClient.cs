using System;
using System.Threading.Tasks;
using StreamJsonRpc;
using System.IO;

namespace Rubberduck.RPC.Platform.Client
{
    /// <summary>
    /// Represents a RPC client service that communicates over an abstract stream.
    /// </summary>
    public abstract class JsonRpcClient<TStream>
        where TStream : Stream
    {
        public IRpcStreamFactory<TStream> RpcStreamFactory { get; }

        public JsonRpcClient(IRpcStreamFactory<TStream> rpcStreamFactory)
        {
            RpcStreamFactory = rpcStreamFactory;
        }

        /// <summary>
        /// Sends a RPC message notifying the server of a client event.
        /// </summary>
        /// <param name="rpcTargetMethod">The name of the RPC method to invoke.</param>
        /// <remarks>
        /// See <c>Rubberduck.RPC.Platform.Metadata.JsonRpcMethods</c> for all server endpoint names.
        /// </remarks>
        public async Task NotifyAsync(string rpcTargetMethod)
        {
            using (var stream = RpcStreamFactory.CreateNew())
            {
                await ConnectAsync(stream);
                using (var rpc = JsonRpc.Attach(stream))
                {
                    await rpc.NotifyAsync(rpcTargetMethod);
                }
            }
        }

        /// <summary>
        /// Sends a RPC message notifying the server of a client event.
        /// </summary>
        /// <param name="method">A delegate that invokes a method against any implementation of <c>IJsonRpcTarget</c>.</param>
        public virtual async Task NotifyAsync<TServerProxy>(Func<TServerProxy, Task> method) where TServerProxy : class, IJsonRpcTarget
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
        /// <typeparam name="TParameter">The class type of the parameter object to be sent to the server.</typeparam>
        /// <param name="rpcTargetMethod">The name of the RPC method to invoke.</param>
        /// <param name="parameter">A parameter object to send along with the notification.</param>
        /// <remarks>
        /// See <c>Rubberduck.RPC.Platform.Metadata.JsonRpcMethods</c> for all server endpoint names.
        /// </remarks>
        public async Task NotifyAsync<TParameter>(string rpcTargetMethod, TParameter parameter)
            where TParameter : class, new()
        {
            using (var stream = RpcStreamFactory.CreateNew())
            {
                await ConnectAsync(stream);
                using (var rpc = JsonRpc.Attach(stream))
                {
                    await rpc.NotifyWithParameterObjectAsync(rpcTargetMethod, parameter);
                }
            }
        }

        /// <summary>
        /// Sends a RPC message notifying the server of a client event.
        /// </summary>
        /// <param name="method">A delegate that invokes a method against any implementation of <c>IJsonRpcTarget</c>.</param>
        /// <param name="parameter">A parameter object to send along with the notification.</param>
        public virtual async Task NotifyAsync<TServerProxy, TParameter>(Func<TServerProxy, TParameter, Task> method, TParameter parameter)
            where TServerProxy : class, IJsonRpcTarget
            where TParameter : class, new()
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
        /// <param name="rpcTargetMethod">The name of the RPC method to invoke.</param>
        /// <returns>The server response.</returns>
        public virtual async Task<TResponse> RequestAsync<TResponse>(string rpcTargetMethod)
            where TResponse : class, new()
        {
            using (var stream = RpcStreamFactory.CreateNew())
            {
                await ConnectAsync(stream);
                using (var rpc = JsonRpc.Attach(stream))
                {
                    return await rpc.InvokeAsync<TResponse>(rpcTargetMethod);
                }
            }
        }

        /// <summary>
        /// Sends a RPC message requesting a response of a given data type from the server.
        /// </summary>
        /// <typeparam name="TParameter">The class type of the parameter object to be sent to the server.</typeparam>
        /// <typeparam name="TResponse">The class type of the response object returned from the server.</typeparam>
        /// <param name="rpcTargetMethod">The name of the RPC method to invoke.</param>
        /// <param name="parameter">A parameter object to send along with the request.</param>
        /// <returns>The server response.</returns>
        public virtual async Task<TResponse> RequestAsync<TParameter, TResponse>(string rpcTargetMethod, TParameter parameter)
            where TParameter : class, new()
            where TResponse : class, new()
        {
            using (var stream = RpcStreamFactory.CreateNew())
            {
                await ConnectAsync(stream);
                using (var rpc = JsonRpc.Attach(stream))
                {
                    return await rpc.InvokeWithParameterObjectAsync<TResponse>(rpcTargetMethod, parameter);
                }
            }
        }

        /// <summary>
        /// Sends a RPC message requesting a response of a given data type from the server.
        /// </summary>
        /// <typeparam name="TResponse">The class type of the response object returned from the server.</typeparam>
        /// <param name="method">A delegate that invokes a method against any implementation of <c>IJsonRpcTarget</c>.</param>
        /// <returns>The server response.</returns>
        public virtual async Task<TResponse> RequestAsync<TServerProxy, TResponse>(Func<TServerProxy, Task<TResponse>> method)
            where TServerProxy : class, IJsonRpcTarget
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
        public virtual async Task<TResponse> RequestAsync<TServerProxy, TParameter, TResponse>(Func<TServerProxy, TParameter, Task<TResponse>> method, TParameter parameter)
            where TServerProxy : class, IJsonRpcTarget
            where TResponse : class, new()
        {
            using (var stream = RpcStreamFactory.CreateNew())
            {
                await ConnectAsync(stream);
                var rpc = JsonRpc.Attach<TServerProxy>(stream);
                return await method.Invoke(rpc, parameter);
            }
        }

        /// <summary>
        /// Asynchronously opens the stream connection.
        /// </summary>
        /// <param name="stream">The stream to connect.</param>
        protected abstract Task ConnectAsync(TStream stream);
    }
}