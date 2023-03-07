using System.Data;

namespace Rubberduck.DatabaseServer.Internal.Abstract
{
    internal interface IDbConnectionProvider : IDisposable
    {
        IDbConnection GetOrCreateConnection();
    }
}
