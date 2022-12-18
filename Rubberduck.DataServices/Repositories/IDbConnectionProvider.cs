using System;
using System.Data;

namespace Rubberduck.DataServices.Repositories
{
    internal interface IDbConnectionProvider : IDisposable
    {
        IDbConnection GetOrCreateConnection();
    }

    internal class DbConnectionProvider : IDbConnectionProvider
    {
        private IDbConnection _connection;

        private bool _disposed;
        public void Dispose()
        {
            if (_disposed) return;

            _connection.Dispose();
            _connection = null;
            _disposed = true;
        }

        public IDbConnection GetOrCreateConnection()
        {
            var connection = _connection;
            if (connection is null)
            {
                // set sqlite connection
                connection.Open();
            }
            return connection;
        }
    }
}
