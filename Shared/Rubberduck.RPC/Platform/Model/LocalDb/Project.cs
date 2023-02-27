using Rubberduck.InternalApi.Model;
using System;
using System.Collections.Generic;

namespace Rubberduck.RPC.Platform.Model.LocalDb
{
    public class Project : DbEntity
    {
        public int OwnerProcessId { get; set; }
        public bool IsCheckedOut { get; set; }

        public int DeclarationId { get; set; }
        public string VBProjectId { get; set; }
        public string Guid { get; set; }
        public int? MajorVersion { get; set; }
        public int? MinorVersion { get; set; }
        public string Path { get; set; }
    }

    public class ProjectInfo : Project
    {
        public DeclarationType DeclarationType { get; set; }
        public string IdentifierName { get; set; }
        public string DocString { get; set; }
        public bool IsUserDefined { get; set; }
    }

    public class ProjectInfoRequestOptions : IQueryOption
    {
        /// <summary>
        /// If provided, results will only include items owned or referenced by the specified process ID.
        /// </summary>
        public int? OwnerProcessId { get; set; }
        /// <summary>
        /// If provided, results will be filtered by ID (primary key).
        /// </summary>
        public int[] Id { get; set; } = Array.Empty<int>();
        /// <summary>
        /// If provided, results will be filtered by GUID (COM libraries).
        /// </summary>
        public string[] Guid { get; set; } = Array.Empty<string>();

        /// <summary>
        /// If provided, results will be filtered by the provided declaration types.
        /// </summary>
        public DeclarationType[] DeclaraitonTypes { get; set; } = Array.Empty<DeclarationType>();
        /// <summary>
        /// If true (default), results will include user-defined items.
        /// </summary>
        public bool IncludeUserDefined { get; set; } = true;
        /// <summary>
        /// If true, results will only include user-defined items. false by default.
        /// </summary>
        /// <remarks>This option takes precedence over <c>IncludeUserDefined</c> if the latter is <c>false</c> but the former is <c>true</c>.</remarks>
        public bool IsUserDefinedOnly { get; set; }

        public string ToWhereClause()
        {
            var criterion = new List<string>();

            if (OwnerProcessId.HasValue)
            {

            }

            return string.Join(" AND ", criterion);
        }
    }
}
