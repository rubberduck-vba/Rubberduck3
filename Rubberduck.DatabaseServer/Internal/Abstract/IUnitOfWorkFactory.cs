namespace Rubberduck.DatabaseServer.Internal.Abstract
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork CreateNew();
    }
}
