using System.Collections.Generic;
using System.Threading.Tasks;
using Rubberduck.Server.LocalDb.Internal.Model;

namespace Rubberduck.Server.LocalDb.Internal.Storage.Abstract
{
    public interface IQueryOptions<TEntity> where TEntity : DbEntity
    {
        Task<IEnumerable<TEntity>> GetByOptionsAsync<TOptions>(TOptions options) where TOptions : class;
    }
}
