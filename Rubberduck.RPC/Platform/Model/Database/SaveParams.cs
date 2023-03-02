using MediatR;
using System.Collections.Generic;

namespace Rubberduck.RPC.Platform.Model.Database
{
    public abstract class SaveParams<TEntity> : IRequest, IRequest<Unit>
    {
        public IEnumerable<TEntity> Entities { get; set; }
    }
}
