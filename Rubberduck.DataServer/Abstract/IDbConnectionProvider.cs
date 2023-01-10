using System;
using System.Data;

namespace Rubberduck.DataServer.Abstract
{
    public interface IDbConnectionProvider : IDisposable
    {
        IDbConnection GetOrCreateConnection();
    }
}
