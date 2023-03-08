namespace Rubberduck.ServerPlatform.RPC.DatabaseServer
{
    public class PrimaryKeyRequestOptions : IQueryOption
    {
        public int Id { get; set; }

        string IQueryOption.ToWhereClause() => $"[Id] = {Id}";
    }
}
