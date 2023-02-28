namespace Rubberduck.Server.Database.Internal.Storage.Abstract
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork CreateNew();
    }
}
