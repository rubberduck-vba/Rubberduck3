using MediatR;
using System;

namespace Rubberduck.RPC.Platform.Model.Database
{
    public abstract class DbEntity : IRequest, IRequest<Unit>
    {
        public int Id { get; set; }
        public DateTime DateInserted { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
