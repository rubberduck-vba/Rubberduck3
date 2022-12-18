using Rubberduck.DataServices.Entities;
using System;
using System.CodeDom;
using System.Data;

namespace Rubberduck.DataServices.Repositories
{
    internal interface IUnitOfWork : IDisposable
    {
        void SaveChanges();
        Repository<TEntity> GetRepository<TEntity>() where TEntity : DbEntity;
    }

    internal class UnitOfWork : IUnitOfWork
    {
        private readonly IDbConnection _db;
        private readonly IDbTransaction _transaction;
        private bool _disposed;
        private bool _committed;

        public UnitOfWork(IDbConnection connection)
        {
            _db = connection;
            _transaction = _db.BeginTransaction();

            Declarations = new DeclarationRepository(connection);
            Projects = new ProjectRepository(connection);
            Modules = new ModuleRepository(connection);
            Members = new MemberRepository(connection);
            Parameters = new ParameterRepository(connection);
            Locals = new LocalRepository(connection);
            IdentifierReferences = new IdentifierReferenceRepository(connection);
        }

        public void SaveChanges()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("This unit of work is already disposed.");
            }

            if (_committed)
            {
                throw new InvalidOperationException("This transaction has already been committed.");
            }

            _transaction.Commit();
            _committed = true;
        }

        public void Dispose()
        {
            _transaction.Dispose();
            _disposed = true;
        }

        public DeclarationRepository Declarations { get; }
        public ProjectRepository Projects { get; }
        public ModuleRepository Modules { get; }
        public MemberRepository Members { get; }
        public ParameterRepository Parameters { get; }
        public LocalRepository Locals { get; }
        public IdentifierReferenceRepository IdentifierReferences { get; }

        public Repository<TEntity> GetRepository<TEntity>()
            where TEntity : DbEntity
        {
            switch (typeof(TEntity))
            {
                case var t when t == typeof(Declaration):
                    return Declarations as Repository<TEntity>;

                case var t when t == typeof(IdentifierReference):
                    return IdentifierReferences as Repository<TEntity>;

                case var t when t == typeof(Project):
                    return Projects as Repository<TEntity>;

                case var t when t == typeof(Module):
                    return Modules as Repository<TEntity>;

                case var t when t == typeof(Member):
                    return Members as Repository<TEntity>;

                case var t when t == typeof(Parameter):
                    return Parameters as Repository<TEntity>;

                case var t when t == typeof(Local):
                    return Locals as Repository<TEntity>;

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
