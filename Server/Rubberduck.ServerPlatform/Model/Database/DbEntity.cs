using System;

namespace Rubberduck.ServerPlatform.Model.LocalDb
{
    public abstract class DbEntity
    {
        public int Id { get; set; }
        public DateTime DateInserted { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
