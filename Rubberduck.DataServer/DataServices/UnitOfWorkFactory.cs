using Rubberduck.DataServer.Abstract;

namespace Rubberduck.Server.DataServices
{
    internal class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly IDbConnectionProvider _provider;

        public UnitOfWorkFactory(IDbConnectionProvider provider)
        {
            _provider = provider;
        }

        public IUnitOfWork CreateNew() => new UnitOfWork(_provider.GetOrCreateConnection());
    }
}
