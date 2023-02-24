using System;
using System.Data;

namespace Rubberduck.Server.LocalDb.Internal.Storage.Abstract
{
    internal interface IDbConnectionProvider : IDisposable
    {
        IDbConnection GetOrCreateConnection();
    }
}
