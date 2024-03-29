﻿namespace Rubberduck.ServerPlatform.Model.Database
{
    /// <summary>
    /// For debugging purposes only.
    /// </summary>
    /// <remarks>
    /// The <c>query/debug</c> endpoint does not exist in a release build.
    /// </remarks>
    public class SqlQuery
    {
        /// <summary>
        /// The raw SQL (SQLite) SELECT statement to execute/debug.
        /// </summary>
        public string RawSqlSelect { get; set; }
    }
}
