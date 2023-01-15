using AustinHarris.JsonRpc;
using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Rubberduck.RPC.Platform
{
    public class OutgoingMessageEventArgs : EventArgs
    {
        public OutgoingMessageEventArgs(string response)
        {
            Response = response;
        }

        public string Response { get; }
    }

    public class JsonRpcBehavior : WebSocketBehavior
    {
        public event EventHandler WebSocketOpened;
        public event EventHandler<CloseEventArgs> WebSocketClosed;
        public event EventHandler<MessageEventArgs> MessageReceived;
        public event EventHandler<OutgoingMessageEventArgs> MessageSent;
        public event EventHandler<ErrorEventArgs> OnErrorResponse;

        protected override void OnOpen()
        {
            base.OnOpen();
            WebSocketOpened?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            var asyncState = new JsonRpcStateAsync(state =>
            {
                var response = ((JsonRpcStateAsync)state).Result;

                // notifications don't send response back
                if (!string.IsNullOrWhiteSpace(response))
                {
                    Send(response);
                    MessageSent?.Invoke(this, new OutgoingMessageEventArgs(response));
                }
            }, null);
            asyncState.JsonRpc = e.Data;

            MessageReceived?.Invoke(this, e);
            JsonRpcProcessor.Process(asyncState);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            base.OnError(e);
            OnErrorResponse?.Invoke(this, e);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            WebSocketClosed?.Invoke(this, e);
        }
    }
}
