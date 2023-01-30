using System;
using System.Data;

namespace Rubberduck.Server.LocalDb.Internal
{
    internal interface IDbConnectionProvider : IDisposable
    {
        IDbConnection GetOrCreateConnection();
    }
}
