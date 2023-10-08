using Rubberduck.ServerPlatform.Model.Database;

namespace Rubberduck.ServerPlatform.Model.LocalDb
{
    public class PrimaryKeyRequestOptions : IQueryOption
    {
        public int Id { get; set; }

        public string ToWhereClause() => $"[Id] = {Id}";
    }
}
