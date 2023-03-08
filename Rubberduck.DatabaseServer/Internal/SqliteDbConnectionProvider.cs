﻿using System.Data;
using Rubberduck.DatabaseServer.Internal.Abstract;
using Microsoft.Data.Sqlite;

namespace Rubberduck.DatabaseServer.Internal.Storage
{
    internal class SqliteDbConnectionProvider : IDbConnectionProvider
    {
        private readonly Lazy<IDbConnection> _lazyConnection
            = new Lazy<IDbConnection>(() =>
            {
                var connection = new SqliteConnection(ServerPlatform.Settings.DatabaseServerConnectionString);
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
