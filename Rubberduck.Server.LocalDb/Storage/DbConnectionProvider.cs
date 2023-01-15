using System;
using System.Data;
using Microsoft.Data.Sqlite;
using Rubberduck.Server.LocalDb;
using Rubberduck.DataServer.Properties;

namespace Rubberduck.Server.Storage
{
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
            if (_disposed) throw new ObjectDisposedException("This provider instance was disposed.");

            var connection = _connection;
            if (connection is null)
            {
                connection = _connection = new SqliteConnection(Settings.Default.LocalDB);
                connection.Open();
            }
            return connection;
        }
    }
}
