using System;
using System.Data;

namespace Rubberduck.Server.Database.Internal.Storage.Abstract
{
    internal interface IDbConnectionProvider : IDisposable
    {
        IDbConnection GetOrCreateConnection();
    }
}
