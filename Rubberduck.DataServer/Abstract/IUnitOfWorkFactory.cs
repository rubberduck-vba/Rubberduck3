namespace Rubberduck.DataServer.Abstract
{
    internal interface IUnitOfWorkFactory
    {
        IUnitOfWork CreateNew();
    }
}
