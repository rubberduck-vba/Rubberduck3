using Rubberduck.InternalApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Rubberduck.RPC.Platform.Model.Database
{
    public class Member : DbEntity
    {
        public Declaration Declaration { get; set; }
        public int DeclarationId { get; set; }
        public int? ImplementsDeclarationId { get; set; }
        public int Accessibility { get; set; }
        public int IsAutoAssigned { get; set; }
        public int IsWithEvents { get; set; }
        public int IsDimStmt { get; set; }
        public string ValueExpression { get; set; }

        public DeclarationAnnotation[] Annotations { get; set; }
        public DeclarationAttribute[] Attributes { get; set; }
        public Parameter[] Parameters { get; set; }
        public Local[] Locals { get; set; }
    }

    public class MemberInfo : Member
    {
        public DeclarationType DeclarationType { get; set; }
        public string IdentifierName { get; set; }
        public string DocString { get; set; }
        public bool IsUserDefined { get; set; }

        public string AsTypeName { get; set; }
        public int? AsTypeDeclarationId { get; set; }
        public bool IsArray { get; set; }
        public string TypeHint { get; set; }

        public int ModuleDeclarationId { get; set; }
        public DeclarationType ModuleDeclarationType { get; set; }
        public string ModuleName { get; set; }
        public string Folder { get; set; }

        public int ProjectDeclarationId { get; set; }
        public string ProjectName { get; set; }
        public string VBProjectId { get; set; }
    }

    public class MemberInfoRequestOptions : IQueryOption
    {
        /// <summary>
        /// If provided, results will be filtered for the provided member declaration IDs
        /// </summary>
        public int[] DeclarationId { get; set; } = Array.Empty<int>();
        /// <summary>
        /// If provided, results will be filtered for the provided module declaration IDs
        /// </summary>
        public int[] ModuleDeclarationId { get; set; } = Array.Empty<int>();

        public int[] ImplementsDeclarationId { get; set; } = Array.Empty<int>();

        public Accessibility[] Accessibility { get; set; } = Array.Empty<Accessibility>();
        public bool IncludeAutoAssigned { get; set; } = true;
        public bool IsAutoAssignedOnly { get; set; }
        public bool IncludeWithEvents { get; set; } = true;
        public bool IsWithEventsOnly { get; set; }
        public bool IncludeDimStmt { get; set; } = true;
        public bool IsDimStmtOnly { get; set; }

        public string ValueExpression { get; set; }

        public string ToWhereClause()
        {
            var criterion = new List<string>();
            if (DeclarationId.Any())
            {
                criterion.Add($" [DeclarationId] IN({string.Join(",", DeclarationId)}) ");
            }

            if (ModuleDeclarationId.Any())
            {
                criterion.Add($" [ModuleDeclarationId] IN ('{string.Join("','", ModuleDeclarationId)}') ");
            }

            if (ImplementsDeclarationId.Any())
            {
                criterion.Add($" [ImplementsDeclarationId] IN ('{string.Join("','", ImplementsDeclarationId)}') ");
            }

            if (Accessibility.Any())
            {
                criterion.Add($" [Accessibility] IN ('{string.Join(",", Accessibility.Cast<int>())}') ");
            }

            if (!IncludeAutoAssigned)
            {
                criterion.Add($" [IsAutoAssigned] = 0 ");
            }
            else if (IsAutoAssignedOnly)
            {
                criterion.Add($" [IsAutoAssigned] = 1 ");
            }

            if (!IncludeWithEvents)
            {
                criterion.Add($" [IsWithEvents] = 0 ");
            }
            else if (IsWithEventsOnly)
            {
                criterion.Add($" [IsWithEvents] = 1 ");
            }

            if (!IncludeDimStmt)
            {
                criterion.Add($" [IsDimStmt] = 0 ");
            }
            else if (IsDimStmtOnly)
            {
                criterion.Add($" [IsDimStmt] = 1 ");
            }

            return string.Join(" AND ", criterion);
        }
    }
}
