﻿namespace Rubberduck.RPC.Platform.Model.Database
{
    public class PrimaryKeyRequestOptions : IQueryOption
    {
        public int Id { get; set; }

        public string ToWhereClause() => $"[Id] = {Id}";
    }
}
