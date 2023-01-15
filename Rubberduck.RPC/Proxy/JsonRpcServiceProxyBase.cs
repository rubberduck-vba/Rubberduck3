namespace Rubberduck.RPC.Platform
{
    /// <summary>
    /// A base class for client-side implementations of server interfaces.
    /// </summary>
    public abstract class JsonRpcServiceProxyBase
    {
        private readonly JsonRpcClient _client;

        protected JsonRpcServiceProxyBase(JsonRpcClient client)
        {
            _client = client;
        }

        protected TResult Request<TResult, TParam>(string method, TParam parameter) 
            where TResult : class
            where TParam : class
        {
            var request = _client.CreateRequest(method, parameter);
            var response = _client.Request<TResult>(request);
            
            return response;
        }

        protected void Notify<TParam>(string method, TParam parameter)
            where TParam : class
        {
            var request = _client.CreateRequest(method, parameter);
            _client.Notify(request);
        }
    }
}
