using System.Data;
using Dapper;
using Rubberduck.DatabaseServer.Internal.Abstract;
using Rubberduck.ServerPlatform.Model.Entities;

namespace Rubberduck.DatabaseServer.Internal.Storage
{
    internal class LocalRepository : Repository<Local>
    {
        public LocalRepository(IDbConnection connection)
            : base(connection) { }

        protected override string Source { get; } = "[Locals]";
        protected override string[] ColumnNames { get; } = new[]
        {
            "DeclarationId",
            "IsAutoAssigned",
            "DeclaredValue",
        };

        public override async Task SaveAsync(Local entity)
        {
            if (entity.Id == default)
            {
                entity.Id = await Database.ExecuteAsync(InsertSql,
                    new
                    {
                        declarationId = entity.DeclarationId,
                        isAutoAssigned = entity.IsAutoAssigned,
                        declaredValue = entity.ValueExpression,
                    });
            }
            else
            {
                await Database.ExecuteAsync(UpdateSql,
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
}
