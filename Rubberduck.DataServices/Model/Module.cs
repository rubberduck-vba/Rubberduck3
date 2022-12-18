
using Rubberduck.DataServices.Repositories;
using System.Threading.Tasks;

namespace Rubberduck.DataServices.Model
{
    internal class Module
    {
        public int Id { get; set; } 

        public Declaration Declaration { get; set; }
        public string Folder { get; set; }

        internal Entities.Module ToEntity()
        {
            return new Entities.Module
            {
                Id = Id,
                DeclarationId = Declaration.Id,
                Folder = Folder,
            };
        }

        internal async Task<Module> FromEntityAsync(Entities.Module entity, Repository<Entities.Declaration> repository)
        {
            var result = new Module
            {
                Id = entity.Id,
                Folder = entity.Folder,
            };

            var declaration = await repository.GetByIdAsync(entity.DeclarationId);
            result.Declaration = await Declaration.FromEntityAsync(declaration, repository);

            return result;
        }
    }
}
