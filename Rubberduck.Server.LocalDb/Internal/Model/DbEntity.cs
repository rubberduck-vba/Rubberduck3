using System;

namespace Rubberduck.Server.LocalDb.Internal.Model
{
    public abstract class DbEntity
    {
        public int Id { get; set; }
        public DateTime DateInserted { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
