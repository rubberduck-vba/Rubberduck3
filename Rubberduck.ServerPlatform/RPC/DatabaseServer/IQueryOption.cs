namespace Rubberduck.ServerPlatform.RPC.DatabaseServer
{
    public interface IQueryOption
    {
        /// <summary>
        /// Gets the SQL representation of an <c>IQueryOption</c> implementation instance.
        /// </summary>
        /// <returns>A string representing the WHERE clause (without the WHERE statement) of a plain SQL (SELECT) query.</returns>
        string ToWhereClause();
    }
}
