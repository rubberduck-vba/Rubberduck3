using Rubberduck.InternalApi.Model;
using System;

namespace Rubberduck.Server.LocalDb.Internal.Model
{
    internal class Module : DbEntity
    {
        public int DeclarationId { get; set; }
        public string Folder { get; set; }
    }

    internal class ModuleInfo : Module
    {
        public DeclarationType DeclarationType { get; set; }
        public string IdentifierName { get; set; }
        public string DocString { get; set; }
        public bool IsUserDefined { get; set; }

        public int ProjectDeclarationId { get; set; }
        public string ProjectName { get; set; }
        public string VBProjectId { get; set; }
    }

    public class ModuleInfoRequestOptions
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
    }

}
