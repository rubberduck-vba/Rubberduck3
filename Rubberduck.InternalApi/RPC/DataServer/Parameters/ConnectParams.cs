namespace Rubberduck.InternalApi.RPC.DataServer.Parameters
{
    public class ConnectParams
    {
        public int ProcessId { get; set; }
        public string Name { get; set; }
    }

    public class DisconnectParams
    {
        public int ProcessId { get; set; }
    }
}
