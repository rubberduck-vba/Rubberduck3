namespace Rubberduck.Server.LocalDb.Internal.Storage.Abstract
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork CreateNew();
    }
}
