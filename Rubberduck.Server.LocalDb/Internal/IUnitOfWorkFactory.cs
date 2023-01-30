namespace Rubberduck.Server.LocalDb.Internal
{
    internal interface IUnitOfWorkFactory
    {
        IUnitOfWork CreateNew();
    }
}
