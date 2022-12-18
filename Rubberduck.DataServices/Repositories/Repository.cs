using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Rubberduck.DataServices.Entities;
using Dapper;

namespace Rubberduck.DataServices.Repositories
{
    internal abstract class Repository<TEntity>
        where TEntity : DbEntity
    {
        protected IDbConnection Database { get; }

        protected Repository(IDbConnection connection)
        {
            Database = connection;
        }

        public abstract Task<TEntity> GetByIdAsync(int id);
        public abstract Task SaveAsync(TEntity entity);
        public abstract Task DeleteAsync(int id);
    }

    internal class DeclarationRepository : Repository<Declaration>
    {
        public DeclarationRepository(IDbConnection connection) 
            : base(connection) { }

        public override async Task DeleteAsync(int id)
        {
            var sql = "DELETE FROM [Declarations] WHERE [Id] = @id;";
            await Database.ExecuteAsync(sql, id);
        }

        public override async Task<Declaration> GetByIdAsync(int id)
        {
            var sql = "SELECT [Id],[DeclarationType],[IsArray],[AsTypeName],[AsTypeDeclarationId],[TypeHint],[IdentifierName],[IsUserDefined],[ParentDeclarationId],[DocString],[Annotations],[Attributes],[DocumentLineStart],[DocumentLineEnd],[ContextStartOffset],[ContextEndOffset],[IdentifierStartOffset],[IdentifierEndOffset] " +
                      "FROM [Declarations] " +
                      "WHERE [Id] = @id;";
            return await Database.QuerySingleOrDefaultAsync<Declaration>(sql, new { id });
        }

        public override async Task SaveAsync(Declaration entity)
        {
            if (entity.Id != default)
            {
                var sql = "INSERT INTO [Declarations] ([DeclarationType],[IsArray],[AsTypeName],[AsTypeDeclarationId],[TypeHint],[IdentifierName],[IsUserDefined],[ParentDeclarationId],[DocString],[Annotations],[Attributes],[DocumentLineStart],[DocumentLineEnd],[ContextStartOffset],[ContextEndOffset],[IdentifierStartOffset],[IdentifierEndOffset]) " +
                          "VALUES (@declarationType, @isArray, @asTypeName, @asTypeDeclarationId, @typeHint, @identifierName, @isUserDefined, @parentDeclarationId, @docString, @annotations, @attributes, @documentLineStart, @documentLineEnd, @contextStartOffset, @contextEndOffset, @identifierStartOffset, @identifierEndOffset) " +
                          "RETURNING [Id];";
                entity.Id = await Database.ExecuteAsync(sql,
                    new
                    {
                        declarationType = entity.DeclarationType,
                        isArray = entity.IsArray,
                        asTypeName = entity.AsTypeName,
                        asTypeDeclarationId = entity.AsTypeDeclarationId,
                        typeHint = entity.TypeHint,
                        identifierName = entity.IdentifierName,
                        isUserDefined = entity.IsUserDefined,
                        parentDeclarationId = entity.ParentDeclarationId,
                        docString = entity.DocString,
                        annotations = entity.Annotations,
                        attributes = entity.Attributes,
                        documentLineStart = entity.DocumentLineStart,
                        documentLineEnd = entity.DocumentLineEnd,
                        contextStartOffset = entity.ContextStartOffset,
                        contextEndOffset = entity.ContextEndOffset,
                        identifierStartOffset = entity.HighlightStartOffset,
                        identifierEndOffset = entity.HighlightEndOffset,
                    });
            }
            else
            {
                var sql = "UPDATE [Declarations] " +
                          "SET [documentLineStart] = @documentLineStart" +
                          "   ,[documentLineEnd] = @documentLineEnd" +
                          "   ,[contextStartOffset] = @contextStartOffset" +
                          "   ,[contextEndOffset] = @contextEndOffset" +
                          "   ,[identifierStartOffset] = @identifierStartOffset" +
                          "   ,[identifierEndOffset] = @identifierEndOffset " +
                          "WHERE [Id] = @id";
                await Database.ExecuteAsync(sql,
                    new
                    {
                        documentLineStart = entity.DocumentLineStart,
                        documentLineEnd = entity.DocumentLineEnd,
                        contextStartOffset = entity.ContextStartOffset,
                        contextEndOffset = entity.ContextEndOffset,
                        identifierStartOffset = entity.HighlightStartOffset,
                        identifierEndOffset = entity.HighlightEndOffset,
                        id = entity.Id,
                    });
            }
        }
    }

    internal class ProjectRepository : Repository<Project>
    {
        public ProjectRepository(IDbConnection connection) 
            : base(connection) { }

        public override async Task DeleteAsync(int id)
        {
            var sql = "DELETE FROM [Projects] WHERE [Id] = @id;";
            await Database.ExecuteAsync(sql, new { id });
        }

        public override async Task<Project> GetByIdAsync(int id)
        {
            var sql = "SELECT [Id],[DeclarationId],[VBProjectId],[Guid],[MajorVersion],[MinorVersion],[Path] " +
                      "FROM [Projects] " +
                      "WHERE [Id] = @id;";
            return await Database.QuerySingleOrDefaultAsync<Project>(sql, new { id });
        }

        public override async Task SaveAsync(Project entity)
        {
            if (entity.Id == default)
            {
                var sql = "INSERT INTO [Projects] ([DeclarationId],[VBProjectId],[Guid],[MajorVersion],[MinorVersion],[Path] " +
                          "VALUES (@declarationId, @vbProjectId, @guid, @majorVersion, @minorVersion, @path) " +
                          "RETURNING [Id];";
                entity.Id = await Database.ExecuteAsync(sql,
                    new
                    {
                        declarationId = entity.DeclarationId,
                        vbProjectId = entity.VBProjectId,
                        guid = entity.Guid,
                        majorVersion = entity.MajorVersion,
                        minorVersion = entity.MinorVersion,
                        path = entity.Path,
                    });
            }
            else
            {
                var sql = "UPDATE [Projects] " +
                          "SET [DeclarationId] = @declarationId" +
                          "   ,[VBProjectId] = @vbProjectId" +
                          "   ,[Guid] = @guid" +
                          "   ,[MajorVersion] = @majorVersion" +
                          "   ,[MinorVersion] = @minorVersion" +
                          "   ,[Path] = @path " +
                          "WHERE [Id] = @id;";
                await Database.ExecuteAsync(sql, 
                    new 
                    {
                        declarationId = entity.DeclarationId,
                        vbProjectId = entity.VBProjectId,
                        guid = entity.Guid,
                        majorVersion = entity.MajorVersion,
                        minorVersion = entity.MinorVersion,
                        path = entity.Path,
                        id = entity.Id,
                    });
            }
        }
    }

    internal class ModuleRepository : Repository<Module>
    {
        public ModuleRepository(IDbConnection connection) 
            : base(connection) { }

        public async Task AddInterface(int moduleId, int implementsModuleId)
        {
            var sql = "IF NOT EXISTS (SELECT * FROM [ModuleInterfaces] WHERE [ModuleId] = @moduleId AND [ImplementsModuleId] = @implementsModuleId" +
                      "INSERT INTO [ModuleInterfaces] ([ModuleId],[ImplementsModuleId]) " +
                      "VALUES (@moduleId, @implementsModuleId);";
            await Database.ExecuteAsync(sql,
                new 
                {
                    moduleId,
                    implementsModuleId,
                });
        }

        public async Task RemoveInterface(int moduleId, int implementsModuleId)
        {
            var sql = "DELETE FROM [ModuleInterfaces] " +
                      "WHERE [ModuleId] = @moduleId " +
                      "AND [ImplementsModuleId] = @implementsModuleId;";
            await Database.ExecuteAsync(sql,
                new
                {
                    moduleId,
                    implementsModuleId,
                });
        }

        public override async Task DeleteAsync(int id)
        {
            var sql = "DELETE FROM [Modules] WHERE [Id] = @id;";
            await Database.ExecuteAsync(sql, new { id });
        }

        public override async Task<Module> GetByIdAsync(int id)
        {
            var sql = "SELECT [Id],[DeclarationId],[Folder] " +
                      "FROM [Modules] " +
                      "WHERE [Id] = @id;";
            return await Database.QuerySingleOrDefaultAsync<Module>(sql, new { id });
        }

        public override async Task SaveAsync(Module entity)
        {
            if (entity.Id == default)
            {
                var sql = "INSERT INTO [Modules] ([DeclarationId],[Folder] " +
                          "VALUES (@declarationId, @folder) " +
                          "RETURNING [Id];";
                entity.Id = await Database.ExecuteAsync(sql,
                    new
                    {
                        declarationId = entity.DeclarationId,
                        folder = entity.Folder,
                    });
            }
            else
            {
                var sql = "UPDATE [Modules] " +
                          "SET [DeclarationId] = @declarationId" +
                          "   ,[Folder] = @folder " +
                          "WHERE [Id] = @id;";
                await Database.ExecuteAsync(sql, new
                {
                    declarationId = entity.DeclarationId,
                    folder = entity.Folder,
                    id = entity.Id,
                });
            }
        }
    }

    internal class MemberRepository : Repository<Member>
    {
        public MemberRepository(IDbConnection connection) 
            : base(connection) { }

        public override async Task DeleteAsync(int id)
        {
            var sql = "DELETE FROM [Members] WHERE [Id] = @id;";
            await Database.ExecuteAsync(sql, new { id });
        }

        public override async Task<Member> GetByIdAsync(int id)
        {
            var sql = "SELECT [Id],[DeclarationId],[ImplementsDeclarationId],[Accessibility],[IsAutoAssigned],[IsWithEvents],[IsDimStmt],[ValueExpression] " +
                      "FROM [Members] " +
                      "WHERE [Id] = @id;";
            return await Database.QuerySingleOrDefaultAsync<Member>(sql, new { id });
        }

        public override async Task SaveAsync(Member entity)
        {
            if (entity.Id == default)
            {
                var sql = "INSERT INTO [Members] ([DeclarationId],[ImplementsDeclarationId],[Accessibility],[IsAutoAssigned],[IsWithEvents],[IsDimStmt],[ValueExpression]) " +
                          "VALUES (@declarationId, @implementsDeclarationId, @accessibility, @isAutoAssigned, @isWithEvents, @isDimStmt, @valueExpression) " +
                          "RETURNING [Id];";
                entity.Id = await Database.ExecuteAsync(sql, 
                    new 
                    {
                        declarationId = entity.DeclarationId,
                        implementsDeclarationId = entity.ImplementsDeclarationId,
                        accessibility = entity.Accessibility,
                        isAutoAssigned = entity.IsAutoAssigned,
                        isWithEvents = entity.IsWithEvents,
                        isDimStmt = entity.IsDimStmt,
                        valueExpression = entity.ValueExpression,
                    });
            }
            else
            {
                var sql = "UPDATE [Members] " +
                          "SET [DeclarationId] = @declarationId" +
                          "   ,[ImplementsDeclarationId] = @implementsDeclarationId" +
                          "   ,[Accessibility] = @accessibility" +
                          "   ,[IsAutoAssigned] = @isAutoAssigned" +
                          "   ,[IsWithEvents] = @isWithEvents" +
                          "   ,[IsDimStmt] = @isDimStmt" +
                          "   ,[ValueExpression] = @valueExpression " +
                          "WHERE [Id] = @id;";
                await Database.ExecuteAsync(sql, 
                    new 
                    {
                        declarationId = entity.DeclarationId,
                        implementsDeclarationId = entity.ImplementsDeclarationId,
                        accessibility = entity.Accessibility,
                        isAutoAssigned = entity.IsAutoAssigned,
                        isWithEvents = entity.IsWithEvents,
                        isDimStmt = entity.IsDimStmt,
                        valueExpression = entity.ValueExpression,
                        id = entity.Id,
                    });
            }
        }
    }

    internal class ParameterRepository : Repository<Parameter>
    {
        public ParameterRepository(IDbConnection connection) 
            : base(connection) { }

        public override async Task DeleteAsync(int id)
        {
            var sql = "DELETE FROM [Parameters] WHERE [Id] = @id;";
            await Database.ExecuteAsync(sql, new { id });
        }

        public override Task<Parameter> GetByIdAsync(int id)
        {
            var sql = "SELECT [Id],[DeclarationId],[Position],[IsParamArray],[IsOptional],[IsByRef],[IsByVal],[IsModifierImplicit],[DefaultValue] " +
                      "FROM [Parameters] " +
                      "WHERE [Id] = @id;";
            return Database.QuerySingleOrDefaultAsync<Parameter>(sql, new { id });
        }

        public override async Task SaveAsync(Parameter entity)
        {
            if (entity.Id == default)
            {
                var sql = "INSERT INTO [Parameters] ([DeclarationId],[Position],[IsParamArray],[IsOptional],[IsByRef],[IsByVal],[IsModifierImplicit],[DefaultValue]) " +
                          "VALUES (@declarationId, @position, @isParamArray, @isOptional, @isByRef, @isByVal, @isModifierImplicit, @defaultValue) " +
                          "RETURNING [Id];";
                entity.Id = await Database.ExecuteAsync(sql,
                    new 
                    {
                        declarationId = entity.DeclarationId,
                        position = entity.Position,
                        isParamArray = entity.IsParamArray,
                        isOptional = entity.IsOptional,
                        isByRef = entity.IsByRef,
                        isByVal = entity.IsByVal,
                        isModifierImplicit = entity.IsModifierImplicit,
                        defaultValue = entity.DefaultValue,
                    });
            }
            else
            {
                var sql = "UPDATE [Parameters] " +
                          "   SET [DeclarationId] = @declarationId" +
                          "      ,[Position] = @position" +
                          "      ,[IsParamArray] = @isParamArray" +
                          "      ,[IsOptional] = @isOptional" +
                          "      ,[IsByRef] = @isByRef" +
                          "      ,[IsByVal] = @isByVal" +
                          "      ,[IsModifierImplicit] = @isModifierImplicit" +
                          "      ,[DefaultValue] = @defaultvalue" +
                          "WHERE [Id] = @id;";
                await Database.ExecuteAsync(sql,
                    new 
                    {
                        declarationId = entity.DeclarationId,
                        position = entity.Position,
                        isParamArray = entity.IsParamArray,
                        isOptional = entity.IsOptional,
                        isByRef = entity.IsByRef,
                        isByVal = entity.IsByVal,
                        isModifierImplicit = entity.IsModifierImplicit,
                        defaultValue = entity.DefaultValue,
                        id = entity.Id,
                    });
            }
        }
    }

    internal class LocalRepository : Repository<Local>
    {
        public LocalRepository(IDbConnection connection) 
            : base(connection) { }

        public override async Task DeleteAsync(int id)
        {
            var sql = "DELETE FROM [Locals] WHERE [Id] = @id;";
            await Database.ExecuteAsync(sql, new { id });
        }

        public override async Task<Local> GetByIdAsync(int id)
        {
            var sql = "SELECT [Id],[DeclarationId],[IsAutoAssigned],[DeclaredValue] " +
                      "FROM [Locals] WHERE [Id] = @id;";
            return await Database.QuerySingleOrDefaultAsync<Local>(sql, new { id });
        }

        public override async Task SaveAsync(Local entity)
        {
            if (entity.Id == default)
            {
                var sql = "INSERT INTO [Locals] ([DeclarationId],[IsAutoAssigned],[DeclaredValue] " +
                          "VALUES (@declarationId, @isAutoAssigned, @declaredValue) " +
                          "RETURNING [Id];";
                entity.Id = await Database.ExecuteAsync(sql,
                    new
                    {
                        declarationId = entity.DeclarationId,
                        isAutoAssigned = entity.IsAutoAssigned,
                        declaredValue = entity.ValueExpression,
                    });
            }
            else
            {
                var sql = "UPDATE [Locals] " +
                          "   SET [DeclarationId] = @declarationId" +
                          "      ,[IsAutoAssigned] = @isAutoAssigned" +
                          "      ,[DeclaredValue] = @declaredValue " +
                          "WHERE [Id] = @id;";
                await Database.ExecuteAsync(sql,
                    new 
                    {
                        declarationId = entity.DeclarationId,
                        isAutoAssigned = entity.IsAutoAssigned,
                        declaredValue = entity.ValueExpression,
                        id = entity.Id,
                    });
            }
        }
    }

    internal class IdentifierReferenceRepository : Repository<IdentifierReference>
    {
        public IdentifierReferenceRepository(IDbConnection connection) 
            : base(connection) { }

        public override async Task DeleteAsync(int id)
        {
            var sql = "DELETE FROM [IdentifierReferences] WHERE [Id] = @id;";
            await Database.ExecuteAsync(sql, new { id });
        }

        public async Task<IEnumerable<IdentifierReference>> GetByReferencedDeclarationIdAsync(int id)
        {
            var sql = "SELECT [Id],[ReferencedDeclarationId],[ParentDeclarationId],[QualifyingReferenceId],[IsAssignmentTarget],[IsExplicitCallStatement],[IsExplicitLetAssignment],[IsSetAssignment],[IsReDim],[IsArrayAccess],[IsProcedureCoercion],[IsIndexedDefaultMemberAccess],[IsNonIndexedDefaultMemberAccess],[IsInnerRecursiveDefaultMemberAccess],[DefaultMemberRescursionDepth],[TypeHint],[Annotations],[DocumentLineStart],[DocumentLineEnd],[ContextStartOffset],[ContextEndOffset],[IdentifierStartOffset],[IdentifierEndOffset] " +
                      "FROM [IdentifierReferences] " +
                      "WHERE [ReferencedDeclarationId] = @id;";
            return await Database.QueryAsync<IdentifierReference>(sql, new { id });
        }

        public override async Task<IdentifierReference> GetByIdAsync(int id)
        {
            var sql = "SELECT [Id],[ReferencedDeclarationId],[ParentDeclarationId],[QualifyingReferenceId],[IsAssignmentTarget],[IsExplicitCallStatement],[IsExplicitLetAssignment],[IsSetAssignment],[IsReDim],[IsArrayAccess],[IsProcedureCoercion],[IsIndexedDefaultMemberAccess],[IsNonIndexedDefaultMemberAccess],[IsInnerRecursiveDefaultMemberAccess],[DefaultMemberRescursionDepth],[TypeHint],[Annotations],[DocumentLineStart],[DocumentLineEnd],[ContextStartOffset],[ContextEndOffset],[IdentifierStartOffset],[IdentifierEndOffset] " +
                      "FROM [IdentifierReferences] " +
                      "WHERE [Id] = @id;";
            return await Database.QuerySingleOrDefaultAsync<IdentifierReference>(sql, new { id });
        }

        public override async Task SaveAsync(IdentifierReference entity)
        {
            if (entity.Id == default)
            {
                var sql = "INSERT INTO [IdentifierReferences] ([ReferencedDeclarationId],[ParentDeclarationId],[QualifyingReferenceId],[IsAssignmentTarget],[IsExplicitCallStatement],[IsExplicitLetAssignment],[IsSetAssignment],[IsReDim],[IsArrayAccess],[IsProcedureCoercion],[IsIndexedDefaultMemberAccess],[IsNonIndexedDefaultMemberAccess],[IsInnerRecursiveDefaultMemberAccess],[DefaultMemberRescursionDepth],[TypeHint],[Annotations],[DocumentLineStart],[DocumentLineEnd],[ContextStartOffset],[ContextEndOffset],[IdentifierStartOffset],[IdentifierEndOffset]) " +
                          "VALUES (@referencedDeclarationId, @parentDeclarationId, @qualifyingReferenceId, @isAssignmentTarget, @isExplicitCallStatement, @isExplicitLetAssignment, @isSetAssignment, @isReDim, @isArrayAccess, @isProcedureCoercion, @isIndexedDefaultMemberAccess, @isNonIndexedDefaultMemberAccess, @isInnerRecursiveDefaultMemberAccess, @defaultMemberRecursionDepth, @typeHint, @annotations, @documentLineStart, @documentLineEnd, @contextStartOffset, @contextEndOffset, @identifierStartOffset, @identifierEndOffset) " +
                          "RETURNING [Id];";
                entity.Id = await Database.ExecuteAsync(sql, new
                {
                    referencedDeclarationId = entity.ReferencedDeclarationId,
                    parentDeclarationId = entity.ParentDeclarationId,
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
                    annotations = entity.Annotations,
                    documentLineStart = entity.DocumentLineStart,
                    documentLineEnd = entity.DocumentLineEnd,
                    contextStartOffset = entity.ContextStartOffset,
                    contextEndOffset = entity.ContextEndOffset,
                    identifierStartOffset = entity.IdentifierStartOffset,
                    identifierEndOffset = entity.IdentifierEndOffset,
                });
            }
            else
            {
                var sql = "UPDATE [IdentifierReferences] " +
                          "   SET [ReferencedDeclarationId] = @referencedDeclarationId" +
                          "      ,[ParentDeclarationId] = @parentDeclarationId" +
                          "      ,[QualifyingReferenceId] = @qualifyingReferenceId" +
                          "      ,[IsAssignmentTarget] = @isAssignmentTarget" +
                          "      ,[IsExplicitCallStatement] = @isExplicitCallStatement" +
                          "      ,[IsExplicitLetAssignment] = @isExplicitLetAssignment" +
                          "      ,[IsSetAssignment] = @isSetAssignment" +
                          "      ,[IsReDim] = @isReDim" +
                          "      ,[IsArrayAccess] = @isArrayAccess" +
                          "      ,[IsProcedureCoercion] = @isProcedureCoercion" +
                          "      ,[IsIndexedDefaultMemberAccess] = @isIndexedDefaultMemberAccess" +
                          "      ,[IsNonIndexedDefaultMemberAccess] = @isNonIndexedDefaultMemberAccess" +
                          "      ,[IsInnerRecursiveDefaultMemberAccess] = @isInnerRecursiveDefaultMemberAccess" +
                          "      ,[DefaultMemberRecursionDepth] = @defaultMemberRecursionDepth" +
                          "      ,[TypeHint] = @typeHint" +
                          "      ,[Annotations] = @annotations" +
                          "      ,[DocumentLineStart] = @documentLineStart" +
                          "      ,[DocumentLineEnd] = @documentLineEnd" +
                          "      ,[ContextStartOffset] = @contextStartOffset" +
                          "      ,[ContextEndOffset] = @contextEndOffset" +
                          "      ,[IdentifierStartOffset] = @identifierStartOffset" +
                          "      ,[IdentifierEndOffset] = @identifierEndOffset " +
                          "WHERE [Id] = @id";
                await Database.ExecuteAsync(sql, new
                {
                    referencedDeclarationId = entity.ReferencedDeclarationId,
                    parentDeclarationId = entity.ParentDeclarationId,
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
                    annotations = entity.Annotations,
                    documentLineStart = entity.DocumentLineStart,
                    documentLineEnd = entity.DocumentLineEnd,
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
