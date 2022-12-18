using Rubberduck.DataServices.Model;
using Rubberduck.DataServices.Repositories;
using Rubberduck.InternalApi.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rubberduck.DataServices
{
    internal class DeclarationService
    {
        private readonly IUnitOfWorkFactory _factory;

        public DeclarationService(IUnitOfWorkFactory factory)
        {
            _factory = factory;
        }

        private DeclarationRepository Declarations(IUnitOfWork uow) 
            => (DeclarationRepository)uow.GetRepository<Entities.Declaration>();

        public async Task SaveAsync(IEnumerable<Declaration> declarations)
        {
            using (var uow = _factory.CreateNew())
            {
                var repository = Declarations(uow);
                foreach (var declaration in declarations)
                {
                    if (declaration.IsMarkedForDeletion)
                    {
                        await repository.DeleteAsync(declaration.Id);
                    }
                    else if (declaration.IsMarkedForUpdate || declaration.Id == default)
                    {
                        await repository.SaveAsync(declaration.ToEntity());
                    }
                }
                uow.SaveChanges();
            }
        }

        private async Task SaveAsync(IUnitOfWork uow, IEnumerable<IdentifierReference> identifierReferences)
        {
            var repository = uow.GetRepository<Entities.IdentifierReference>();
            foreach (var reference in identifierReferences)
            {
                await repository.SaveAsync(reference.ToEntity());
            }
        }
    }
}
