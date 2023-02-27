using System.Data;
using System.Threading.Tasks;
using Dapper;
using Rubberduck.RPC.Platform.Model.LocalDb;
using Rubberduck.Server.LocalDb.Internal.Storage.Abstract;

namespace Rubberduck.Server.LocalDb.Internal.Storage
{
    internal class MemberRepository : Repository<Member>
    {
        public MemberRepository(IDbConnection connection) 
            : base(connection) { }

        protected override string Source { get; } = "[Members]";
        protected override string[] ColumnNames { get; } = new[]
        {
            "DeclarationId",
            "ImplementsDeclarationId",
            "Accessibility",
            "IsAutoAssigned",
            "IsWithEvents",
            "IsDimStmt",
            "ValueExpression",
        };

        public async Task<Member> GetByDeclarationIdAsync(int declarationId)
        {
            return await Database.QuerySingleOrDefaultAsync($"SELECT [Id],{NonIdentityColumns} FROM {Source} WHERE [DeclarationId] = @declarationId;", new { declarationId });
        }

        public override async Task SaveAsync(Member entity)
        {
            if (entity.Id == default)
            {
                entity.Id = await Database.ExecuteAsync(InsertSql, new 
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
                await Database.ExecuteAsync(UpdateSql, new 
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
}
