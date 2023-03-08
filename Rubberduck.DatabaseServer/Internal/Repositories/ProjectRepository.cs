using System.Data;
using Dapper;
using Rubberduck.DatabaseServer.Internal.Abstract;
using Rubberduck.ServerPlatform.Model.Entities;

namespace Rubberduck.DatabaseServer.Internal.Storage
{
    internal class ProjectRepository : Repository<Project>
    {
        public ProjectRepository(IDbConnection connection) 
            : base(connection) { }

        protected override string Source { get; } = "[Projects]";
        protected override string[] ColumnNames { get; } = new[]
        { 
            "DeclarationId",
            "VBProjectId",
            "Guid",
            "MajorVersion",
            "MinorVersion",
            "Path",
        };

        public override Task<IEnumerable<Project>> GetByOptionsAsync<TOptions>(TOptions options)
        {
            throw new System.NotImplementedException();
        }

        public override async Task SaveAsync(Project entity)
        {
            if (entity.Id == default)
            {
                entity.Id = await Database.ExecuteAsync(InsertSql, new
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
                await Database.ExecuteAsync(UpdateSql, new 
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
}
