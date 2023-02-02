namespace Rubberduck.Server.LocalDb.Internal
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork CreateNew();
    }
}
