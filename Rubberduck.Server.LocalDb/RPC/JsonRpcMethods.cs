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

        /// <summary>
        /// Requests information about a particular project, by project ID.
        /// </summary>
        public const string QueryProjectInfo = "query/projectInfo";

        /// <summary>
        /// Requests information about all projects, for further server-side (in-memory) processing.
        /// </summary>
        public const string QueryProjects = "query/projects";

        /// <summary>
        /// Requests information about a particular module, by ID.
        /// </summary>
        public const string QueryModuleInfo = "query/moduleInfo";

        /// <summary>
        /// Requests information about all modules under a particular project ID, for further server-side (in-memory) processing.
        /// </summary>
        public const string QueryModules = "query/modules";

        /// <summary>
        /// Requests information about a particular member, by ID.
        /// </summary>
        public const string QueryMemberInfo = "query/memberInfo";

        /// <summary>
        /// Requests information about all members under a particular module ID, for further server-side (in-memory) processing.
        /// </summary>
        public const string QueryMembers = "query/members";

        /// <summary>
        /// Requests information about a particular member, by ID.
        /// </summary>
        public const string QueryParameterInfo = "query/parameterInfo";

        /// <summary>
        /// Requests information about all parameters under a particular member ID, for further server-side (in-memory) processing.
        /// </summary>
        public const string QueryParameters = "query/parameters";
        #endregion
    }
}
