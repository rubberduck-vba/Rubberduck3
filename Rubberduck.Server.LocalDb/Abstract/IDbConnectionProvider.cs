using System;
using System.Data;

namespace Rubberduck.Server.LocalDb.Abstract
{
    public interface IDbConnectionProvider : IDisposable
    {
        IDbConnection GetOrCreateConnection();
    }
}
