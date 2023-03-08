using MediatR;
using Rubberduck.ServerPlatform.Model.Entities;

namespace Rubberduck.ServerPlatform.Model
{
    public class SaveParam<TEntity> : IRequest, IRequest<SaveResult>
        where TEntity : DbEntity, new()
    {
        public IEnumerable<TEntity> Entities { get; set; } = Enumerable.Empty<TEntity>();
    }

    public class SaveResult
    {
        public static SaveResult Success { get; } = new SaveResult { Saved = true };

        public bool Saved { get; set; }
    }
}
