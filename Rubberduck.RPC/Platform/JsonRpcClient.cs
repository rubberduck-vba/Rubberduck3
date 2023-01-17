using AustinHarris.JsonRpc;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using WebSocketSharp;

namespace Rubberduck.RPC.Platform
{
    public abstract class JsonRpcClient
    {
        public int MaximumRequestId { get; set; } = int.MaxValue - 1;
        public TimeSpan ResponseTimeout { get; set; } = TimeSpan.FromMinutes(10);

        private static int _requestId = 0;

        private static readonly ConcurrentDictionary<string, TaskCompletionSource<JsonResponse>> _responses
            = new ConcurrentDictionary<string, TaskCompletionSource<JsonResponse>>();

        private readonly WebSocket _socket;

        public JsonRpcClient(WebSocket socket)
        {
            _socket = socket;
            _socket.OnMessage += ProcessMessage;
        }

        private void ProcessMessage(object sender, MessageEventArgs e)
        {
            if (e.IsPing)
            {
                // TODO log ping
                return;
            }

            try
            {
                var response = JsonSerializer.Deserialize<JsonResponse>(e.Data);
                if (response.Error != null)
                {
                    // TODO log error info
                }

                var responseId = Convert.ToString(response.Id);
                if (_responses.TryGetValue(responseId, out var completionSource))
                {
                    completionSource.TrySetResult(response);
                }
                else
                {
                    // TODO log unexpected response ID
                }
            }
            catch
            {
                // TODO log exception
            }
        }

        public JsonRequest CreateRequest(string method, object parameters)
        {
            var nextId = Interlocked.Increment(ref _requestId);
            if (nextId > MaximumRequestId)
            {
                Interlocked.Exchange(ref _requestId, 0);
                nextId = Interlocked.Increment(ref _requestId);
            }

            return new JsonRequest(method, parameters, nextId);
        }

        public TResponse Request<TResponse>(JsonRequest request) where TResponse : class
        {
            var completionSource = new TaskCompletionSource<JsonResponse>();
            var requestId = request.Id.ToString();

            try
            {
                var jsonRequest = JsonSerializer.Serialize(request);
                _responses.TryAdd(requestId, completionSource);
                _socket.Send(jsonRequest);

                var task = completionSource.Task;
                Task.WaitAll(new[] { task }, ResponseTimeout);

                if (task.IsCompleted)
                {
                    var response = task.Result;
                    if (response.Error is null)
                    {
                        return (TResponse)response.Result;
                    }

                    throw response.Error;
                }
                else
                {
                    throw new TimeoutException();
                }
            }
            finally
            {
                _responses.TryRemove(requestId, out _);
            }
        }

        public void Notify(JsonRequest request)
        {
            var jsonRequest = JsonSerializer.Serialize(request);
            _socket.Send(jsonRequest);
        }
    }
}
