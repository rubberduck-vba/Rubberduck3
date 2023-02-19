namespace Rubberduck.Server.LocalDb
{
    internal static class LocalDbServerConfiguration
    {
        /// <summary>
        /// Gets the default configuration for this server, taking the specified <see cref="StartupOptions"/> into account.
        /// </summary>
        public static LocalDbServerCapabilities Default(StartupOptions startupOptions)
        {
            return new LocalDbServerCapabilities
            {
                /* TODO */
            };
        }
    }
}
