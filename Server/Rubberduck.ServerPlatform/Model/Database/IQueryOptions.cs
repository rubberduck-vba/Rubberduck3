﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rubberduck.ServerPlatform.Model.LocalDb;

namespace Rubberduck.ServerPlatform.Model.Database
{
    public interface IQueryOption
    {
        string ToWhereClause();
    }

    public interface IQueryOptions<TEntity> where TEntity : DbEntity
    {
        Task<TEntity> GetByIdAsync(int id);
        Task<IEnumerable<TEntity>> GetByIdAsync(params int[] id);
        Task<IEnumerable<TEntity>> GetByOptionsAsync<TOptions>(TOptions options) where TOptions : IQueryOption;
    }
}
