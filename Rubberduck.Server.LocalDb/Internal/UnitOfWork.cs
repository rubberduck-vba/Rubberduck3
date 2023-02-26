using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Platform.Model.LocalDb;
using Rubberduck.Server.LocalDb.Internal.Storage.Abstract;

namespace Rubberduck.Server.LocalDb.Internal.Storage
{
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

            ProjectsView = new ProjectsView(connection);
            ModulesView = new ModulesView(connection);
            MembersView = new MembersView(connection);
            ParametersView = new ParametersView(connection);
            LocalsView = new LocalsView(connection);
            IdentifierReferencesView = new IdentifierReferencesView(connection);
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
        public DeclarationAnnotationRepository DeclarationAnnotations { get; }
        public DeclarationAttributeRepository DeclarationAttributes { get; }
        public IdentifierReferenceAnnotationRepository IdentifierReferenceAnnotations { get; }


        public ProjectRepository Projects { get; }
        public ModuleRepository Modules { get; }
        public MemberRepository Members { get; }
        public ParameterRepository Parameters { get; }
        public LocalRepository Locals { get; }
        public IdentifierReferenceRepository IdentifierReferences { get; }

        public ProjectsView ProjectsView { get; }
        public ModulesView ModulesView { get; }
        public MembersView MembersView { get; }
        public ParametersView ParametersView { get; }
        public LocalsView LocalsView { get; }
        public IdentifierReferencesView IdentifierReferencesView { get; }

        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string sql, object parameters)
        {
            return await _db.QueryAsync<TEntity>(sql, parameters);
        }

        public async Task<TEntity> QuerySingleOrDefaultAsync<TEntity>(string sql, object parameters)
        {
            return await _db.QuerySingleOrDefaultAsync<TEntity>(sql, parameters);
        }

        public Repository<TEntity> GetRepository<TEntity>() where TEntity : DbEntity
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

        public IQueryOptions<TEntity> GetView<TEntity>() where TEntity : DbEntity
        {
            switch (typeof(TEntity))
            {
                case var t when t == typeof(ProjectInfo):
                    return ProjectsView as View<TEntity>;

                case var t when t == typeof(ModuleInfo):
                    return ModulesView as View<TEntity>;

                case var t when t == typeof(MemberInfo):
                    return MembersView as View<TEntity>;

                case var t when t == typeof(ParameterInfo):
                    return ParametersView as View<TEntity>;

                case var t when t == typeof(LocalInfo):
                    return LocalsView as View<TEntity>;

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
