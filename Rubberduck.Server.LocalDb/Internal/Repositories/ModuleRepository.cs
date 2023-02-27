using System.Data;
using System.Threading.Tasks;
using Dapper;
using Rubberduck.RPC.Platform.Model.LocalDb;
using Rubberduck.Server.LocalDb.Internal.Storage.Abstract;

namespace Rubberduck.Server.LocalDb.Internal.Storage
{
    internal class ModuleRepository : Repository<Module>
    {
        public ModuleRepository(IDbConnection connection) 
            : base(connection) { }

        protected override string Source { get; } = "[Modules]";
        protected override string[] ColumnNames => new[] { "DeclarationId","Folder" };

        public async Task AddInterface(int moduleId, int implementsModuleId)
        {
            var sql = "IF NOT EXISTS (SELECT * FROM [ModuleInterfaces] WHERE [ModuleId] = @moduleId AND [ImplementsModuleId] = @implementsModuleId" +
                      "INSERT INTO [ModuleInterfaces] ([ModuleId],[ImplementsModuleId]) " +
                      "VALUES (@moduleId, @implementsModuleId);";
            await Database.ExecuteAsync(sql, new 
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
            await Database.ExecuteAsync(sql, new
                {
                    moduleId,
                    implementsModuleId,
                });
        }

        public override async Task SaveAsync(Module entity)
        {
            if (entity.Id == default)
            {
                entity.Id = await Database.ExecuteAsync(InsertSql, new
                    {
                        declarationId = entity.DeclarationId,
                        folder = entity.Folder,
                    });
            }
            else
            {
                await Database.ExecuteAsync(UpdateSql, new
                    {
                        declarationId = entity.DeclarationId,
                        folder = entity.Folder,
                        id = entity.Id,
                    });
            }
        }
    }
}
