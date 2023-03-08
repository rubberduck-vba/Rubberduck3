namespace Rubberduck.ServerPlatform.Model
{
    public class ClientProcessInfo
    {
        /// <summary>
        /// The name of the client.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The client application version.
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// The ID of the client process.
        /// </summary>
        public int ProcessId { get; set; }
    }
}
