using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.Server.LocalDb.RPC
{
    internal static class JsonRpcMethods
    {

    #region server
        /// <summary>
        /// Connects a client to the LocalDb server.
        /// </summary>
        public const string Connect = "connect";

        /// <summary>
        /// Disconnects a client from the LocalDb server.
        /// </summary>
        public const string Disconnect = "disconnect";

        /// <summary>
        /// Gets information about the server.
        /// </summary>
        public const string Info = "info";
    #endregion

    #region writing
        /// <summary>
        /// Saves (insert/update) one or more project declarations to the database.
        /// </summary>
        public const string SaveProject = "save/project";

        /// <summary>
        /// Saves (insert/update) one or more project references to the database.
        /// </summary>
        /// <remarks>
        /// Project references refer to another project declaration, which may be another user project, or a referenced type library.
        /// </remarks>
        public const string SaveProjectReference = "save/projectReference";
        /// <summary>
        /// Saves (insert/update) one or more module declarations to the database.
        /// </summary>
        public const string SaveModule = "save/module";

        /// <summary>
        /// Saves (insert/update) one or more member declarations to the database.
        /// </summary>
        public const string SaveMember = "save/member";

        /// <summary>
        /// Saves (insert/update) one or more parameter declarations to the database.
        /// </summary>
        public const string SaveParameter = "save/parameter";

        /// <summary>
        /// Saves (insert/update) one or more local declarations to the database.
        /// </summary>
        public const string SaveLocal = "save/local";

        /// <summary>
        /// Saves (insert/update) one or more identifier references to the database.
        /// </summary>
        public const string SaveIdentifierReference = "save/identifierReference";
    #endregion

    #region reading

        /// <summary>
        /// Sends a plain SQL query to the database, for debugging.
        /// </summary>
        public const string DebugSqlSelect = "query/debug";

        /*TODO*/
        
    #endregion
    }
}
