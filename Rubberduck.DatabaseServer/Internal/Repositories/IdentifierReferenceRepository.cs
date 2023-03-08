﻿using System.Data;
using Dapper;
using Rubberduck.DatabaseServer.Internal.Abstract;
using Rubberduck.ServerPlatform.Model.Entities;

namespace Rubberduck.DatabaseServer.Internal.Storage
{
    internal class IdentifierReferenceRepository : Repository<IdentifierReferenceInfo>
    {
        public IdentifierReferenceRepository(IDbConnection connection) 
            : base(connection) { }

        protected override string Source { get; } = "[IdentifierReferences]";
        protected override string[] ColumnNames { get; } = new[]
        {
            "ReferencedDeclarationId",
            "ParentDeclarationId",
            "QualifyingReferenceId",
            "IsAssignmentTarget",
            "IsExplicitCallStatement",
            "IsExplicitLetAssignment",
            "IsSetAssignment",
            "IsReDim",
            "IsArrayAccess",
            "IsProcedureCoercion",
            "IsIndexedDefaultMemberAccess",
            "IsNonIndexedDefaultMemberAccess",
            "IsInnerRecursiveDefaultMemberAccess",
            "DefaultMemberRescursionDepth",
            "TypeHint",
            "ContextStartOffset",
            "ContextEndOffset",
            "IdentifierStartOffset",
            "IdentifierEndOffset",
        };

        public async Task<IEnumerable<IdentifierReferenceInfo>> GetByReferencedDeclarationIdAsync(int id)
        {
            var sql = $"SELECT [Id],{NonIdentityColumns} FROM {Source} WHERE [ReferencedDeclarationId]=@id;";
            return await Database.QueryAsync<IdentifierReferenceInfo>(sql, new { id });
        }


        public override async Task SaveAsync(IdentifierReferenceInfo entity)
        {
            if (entity.Id == default)
            {
                entity.Id = await Database.ExecuteAsync(InsertSql, new
                {
                    referencedDeclarationId = entity.ReferenceDeclarationId,
                    parentDeclarationId = entity.MemberDeclarationId ?? entity.ModuleDeclarationId,
                    qualifyingReferenceId = entity.QualifyingReferenceId,
                    isAssignmentTarget = entity.IsAssignmentTarget,
                    isExplicitCallStatement = entity.IsExplicitCallStatement,
                    isExplicitLetAssignment = entity.IsExplicitLetAssignment,
                    isSetAssignment = entity.IsSetAssignment,
                    isReDim = entity.IsReDim,
                    isArrayAccess = entity.IsArrayAccess,
                    isProcedureCoercion = entity.IsProcedureCoercion,
                    isIndexedDefaultMemberAccess = entity.IsIndexedDefaultMemberAccess,
                    isNonIndexedDefaultMemberAccess = entity.IsNonIndexedDefaultMemberAccess,
                    isInnerRecursiveDefaultMemberAccess = entity.IsInnerRecursiveDefaultMemberAccess,
                    defaultMemberRecursionDepth = entity.DefaultMemberRecursionDepth,
                    typeHint = entity.TypeHint,
                    contextStartOffset = entity.ContextStartOffset,
                    contextEndOffset = entity.ContextEndOffset,
                    identifierStartOffset = entity.IdentifierStartOffset,
                    identifierEndOffset = entity.IdentifierEndOffset,
                });
            }
            else
            {
                await Database.ExecuteAsync(UpdateSql, new
                {
                    referencedDeclarationId = entity.ReferenceDeclarationId,
                    parentDeclarationId = entity.MemberDeclarationId ?? entity.ModuleDeclarationId,
                    qualifyingReferenceId = entity.QualifyingReferenceId,
                    isAssignmentTarget = entity.IsAssignmentTarget,
                    isExplicitCallStatement = entity.IsExplicitCallStatement,
                    isExplicitLetAssignment = entity.IsExplicitLetAssignment,
                    isSetAssignment = entity.IsSetAssignment,
                    isReDim = entity.IsReDim,
                    isArrayAccess = entity.IsArrayAccess,
                    isProcedureCoercion = entity.IsProcedureCoercion,
                    isIndexedDefaultMemberAccess = entity.IsIndexedDefaultMemberAccess,
                    isNonIndexedDefaultMemberAccess = entity.IsNonIndexedDefaultMemberAccess,
                    isInnerRecursiveDefaultMemberAccess = entity.IsInnerRecursiveDefaultMemberAccess,
                    defaultMemberRecursionDepth = entity.DefaultMemberRecursionDepth,
                    typeHint = entity.TypeHint,
                    contextStartOffset = entity.ContextStartOffset,
                    contextEndOffset = entity.ContextEndOffset,
                    identifierStartOffset = entity.IdentifierStartOffset,
                    identifierEndOffset = entity.IdentifierEndOffset,
                    id = entity.Id,
                });
            }
        }
    }
}