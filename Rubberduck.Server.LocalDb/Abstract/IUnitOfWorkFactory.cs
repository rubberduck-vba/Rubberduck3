namespace Rubberduck.Server.LocalDb.Abstract
{
    internal interface IUnitOfWorkFactory
    {
        IUnitOfWork CreateNew();
    }
}
