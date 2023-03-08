﻿using Rubberduck.ServerPlatform.Model.Entities;

namespace Rubberduck.DatabaseServer.Internal.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
        Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string sql, object parameters);
        Task<TEntity> QuerySingleOrDefaultAsync<TEntity>(string sql, object parameters);

        void SaveChanges();
        Repository<TEntity> GetRepository<TEntity>() where TEntity : DbEntity;
        View<TEntity> GetView<TEntity>() where TEntity : DbEntity;
    }
}
