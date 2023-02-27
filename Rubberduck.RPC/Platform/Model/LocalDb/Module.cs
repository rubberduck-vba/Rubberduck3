using Rubberduck.InternalApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.RPC.Platform.Model.LocalDb
{
    public class Module : DbEntity
    {
        public int DeclarationId { get; set; }
        public string Folder { get; set; }
    }

    public class ModuleInfo : Module
    {
        public DeclarationType DeclarationType { get; set; }
        public string IdentifierName { get; set; }
        public string DocString { get; set; }
        public bool IsUserDefined { get; set; }

        public int ProjectDeclarationId { get; set; }
        public string ProjectName { get; set; }
        public string VBProjectId { get; set; }
    }

    public class ModuleInfoRequestOptions : IQueryOption
    {
        /// <summary>
        /// If provided, results will be filtered for the provided declaration IDs
        /// </summary>
        public int[] DeclarationId { get; set; } = Array.Empty<int>();

        /// <summary>
        /// If provided, results will be filtered for the specified (workspace/virtual/@folder) folders.
        /// </summary>
        public string[] Folder { get; set; } = Array.Empty<string>();

        /// <summary>
        /// If provided, results will be filtered by the provided declaration types.
        /// </summary>
        public DeclarationType[] DeclarationTypes { get; set; } = Array.Empty<DeclarationType>();
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
            if (DeclarationId.Any())
            {
                criterion.Add($" [DeclarationId] IN({string.Join(",", DeclarationId)}) ");
            }

            if (Folder.Any())
            {
                criterion.Add($" [Folder] IN ('{string.Join("','", Folder)}') ");
            }

            if (DeclarationTypes.Any())
            {
                criterion.Add($" [DeclarationType] IN ('{string.Join("','", DeclarationTypes)}') ");
            }

            if (!IncludeUserDefined)
            {
                criterion.Add($" [IsUserDefined] = 0 ");
            }
            else if (IsUserDefinedOnly)
            {
                criterion.Add($" [IsUserDefined] = 1 ");
            }

            return string.Join(" AND ", criterion);
        }
    }

}
