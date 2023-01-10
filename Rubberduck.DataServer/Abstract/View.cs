using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Rubberduck.DataServer.Abstract
{
    internal abstract class View<TEntity>
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

        protected static string Quoted(string identifier) // TODO grab the Span<string> opportunity
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

        public virtual async Task<IEnumerable<TEntity>> GetByIdAsync(IEnumerable<int> ids)
        {
            var sql = $"SELECT {Columns} FROM {Quoted(Source)} WHERE [Id] IN ({string.Join(",", ids)});";
            return await Database.QueryAsync<TEntity>(sql);
        }
    }
}
