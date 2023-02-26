using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Platform.Model.LocalDb;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rubberduck.Server.LocalDb.Internal.Storage.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
        Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string sql, object parameters);
        Task<TEntity> QuerySingleOrDefaultAsync<TEntity>(string sql, object parameters);

        void SaveChanges();
        Repository<TEntity> GetRepository<TEntity>() where TEntity : DbEntity;
        IQueryOptions<TEntity> GetView<TEntity>() where TEntity : DbEntity;
    }
}
