using System;

namespace Rubberduck.Server.LocalDb.Internal
{
    public abstract class DbEntity
    {
        public int Id { get; set; }
        public DateTime DateInserted { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
