﻿using System.Data;
using Dapper;
using Rubberduck.ServerPlatform.Model.Entities;
using Rubberduck.ServerPlatform.RPC.DatabaseServer;

namespace Rubberduck.DatabaseServer.Internal.Abstract
{
    public abstract class View<TEntity>
        where TEntity : DbEntity
    {
        protected IDbConnection Database { get; }

        protected View(IDbConnection connection)
        {
            Database = connection;

            Columns = string.Join(",", ColumnNames.Select(Quoted));
        }

        protected abstract string[] ColumnNames { get; }
        protected abstract string Source { get; }
        protected string Columns { get; }

        protected static string Quoted(string identifier) 
        {
            var value = identifier ?? throw new ArgumentNullException(nameof(identifier));

            if (value.Count(c => c == '[') != value.Count(c => c == ']'))
            {
                throw new FormatException("Opening '[' and closing ']' brackets are not balanced.");
            }

            if (value.Length > 2 && value[0] == '[' && value[value.Length - 1] == ']')
            {
                return value;
            }

            return $"[{value}]";
        }

        public virtual async Task<TEntity> GetByIdAsync(int id)
        {
            var sql = $"SELECT {Columns} FROM {Quoted(Source)} WHERE [Id]=@id;";
            return await Database.QuerySingleOrDefaultAsync<TEntity>(sql, new { id });
        }

        public virtual async Task<IEnumerable<TEntity>> GetByIdAsync(params int[] ids)
        {
            var sql = $"SELECT {Columns} FROM {Quoted(Source)} WHERE [Id] IN ({string.Join(",", ids)});";
            return await Database.QueryAsync<TEntity>(sql);
        }

        public virtual async Task<IEnumerable<TEntity>> GetByOptionsAsync<TOptions>(TOptions options) where TOptions : IQueryOption
        {
            var sql = $"SELECT {Columns} FROM {Quoted(Source)} WHERE {options.ToWhereClause()}";
            return await Database.QueryAsync<TEntity>(sql);
        }

        public virtual async Task<int> GetCountAsync()
        {
            var sql = $"SELECT COUNT(*) FROM {Quoted(Source)}";
            return await Database.QuerySingleAsync<int>(sql);
        }
    }
}
