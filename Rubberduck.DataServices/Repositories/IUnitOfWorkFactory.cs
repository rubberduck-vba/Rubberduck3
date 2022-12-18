namespace Rubberduck.DataServices.Repositories
{
    internal interface IUnitOfWorkFactory
    {
        IUnitOfWork CreateNew();
    }

    internal class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly IDbConnectionProvider _provider;

        public UnitOfWorkFactory(IDbConnectionProvider provider)
        {
            _provider = provider;
        }

        public IUnitOfWork CreateNew()
            => new UnitOfWork(_provider.GetOrCreateConnection());
    }
}
