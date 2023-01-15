namespace Rubberduck.InternalApi.RPC.DataServer.Response
{
    public class ConnectResult
    {
        public bool Connected { get; set; }
    }

    public class DisconnectResult
    {
        public bool Disconnected { get; set; }
        public bool ShuttingDown { get; set; }
    }
}
