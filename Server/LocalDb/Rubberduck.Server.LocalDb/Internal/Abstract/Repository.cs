using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Rubberduck.RPC.Platform.Model.LocalDb;

namespace Rubberduck.Server.LocalDb.Internal.Storage.Abstract
{
    public abstract class Repository<TEntity> : View<TEntity>
        where TEntity : DbEntity
    {
        protected Repository(IDbConnection connection) 
            : base(connection)
        {
            NonIdentityColumns = string.Join(",", ColumnNames.Select(Quoted));
            NonIdentityValues = string.Join(",", ColumnNames.Select(c => $"@{char.ToLowerInvariant(c[0])}{c.Substring(1, c.Length - 1)}"));
            NonIdentityAssignments = string.Join(",", ColumnNames.Select(c => $"{Quoted(c)} = @{char.ToLowerInvariant(c[0])}{c.Substring(1, c.Length - 1)}"));
        }

        protected string NonIdentityColumns { get; }
        protected string NonIdentityValues { get; }
        protected string NonIdentityAssignments { get; }

        protected string InsertSql => $"INSERT INTO {Quoted(Source)} ([DateInserted],{NonIdentityColumns}) VALUES (CURRENT_TIMESTAMP,{NonIdentityValues}) RETURNING [Id];";
        protected string UpdateSql => $"UPDATE {Quoted(Source)} SET [DateUpdated]=CURRENT_TIMESTAMP,{NonIdentityAssignments} WHERE [Id]=@id;";

        public abstract Task SaveAsync(TEntity entity);

        public virtual async Task DeleteAsync(int id)
        {
            var sql = $"DELETE FROM {Source} WHERE [Id]=@id;";
            await Database.ExecuteAsync(sql, id);
        }

        public override async Task<TEntity> GetByIdAsync(int id)
        {
            var sql = $"SELECT [Id],{NonIdentityColumns} FROM {Quoted(Source)} WHERE [Id]=@id;";
            return await Database.QuerySingleOrDefaultAsync<TEntity>(sql, new { id });
        }
    }
}
