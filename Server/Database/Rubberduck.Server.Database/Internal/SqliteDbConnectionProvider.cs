using System;
using System.Data;
using Microsoft.Data.Sqlite;
using Rubberduck.Server.Database.Internal.Storage.Abstract;
using Rubberduck.Server.Database.Properties;

namespace Rubberduck.Server.Database.Internal.Storage
{
    internal class SqliteDbConnectionProvider : IDbConnectionProvider
    {
        private readonly Lazy<IDbConnection> _lazyConnection
            = new Lazy<IDbConnection>(() =>
            {
                var connection = new SqliteConnection(Settings.Default.Database);
                connection.Open();
                return connection;
            });

        private bool _disposed;
        public void Dispose()
        {
            if (_disposed) return;

            if (_lazyConnection.IsValueCreated)
            {
                _lazyConnection.Value.Dispose();
            }

            _disposed = true;
        }

        public IDbConnection GetOrCreateConnection()
        {
            if (_disposed) throw new ObjectDisposedException("This provider instance was disposed.");
            return _lazyConnection.Value;
        }
    }
}
