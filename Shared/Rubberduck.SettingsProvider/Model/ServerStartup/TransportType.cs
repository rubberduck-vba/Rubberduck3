namespace Rubberduck.SettingsProvider.Model.ServerStartup
{
    public enum TransportType
    {
        /// <summary>
        /// Client/server messages are exchanged through standard input/output streams.
        /// </summary>
        StdIO,

        /// <summary>
        /// Client/server messages are exchanged through a bidirectional named pipe stream.
        /// </summary>
        Pipe,

        //RawSocket
    }
}