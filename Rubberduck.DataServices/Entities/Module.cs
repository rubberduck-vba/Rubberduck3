using ProtoBuf;
using Rubberduck.DataServices.Repositories;
using System.Threading.Tasks;

namespace Rubberduck.DataServices.Entities
{
    internal class Module : DbEntity
    {
        public int DeclarationId { get; set; }
        public string Folder { get; set; }

        internal static Module FromModel(InternalApi.Model.RPC.Module model)
        {
            return new Module
            {
                Id = model.Id,
                DeclarationId = model.Declaration.Id,
                Folder = model.Folder,
            };
        }

        internal async Task<InternalApi.Model.RPC.Module> ToModelAsync(Repository<Declaration> repository)
        {
            var result = new InternalApi.Model.RPC.Module
            {
                Id = Id,
                Folder = Folder,
            };

            var declaration = await repository.GetByIdAsync(DeclarationId);
            result.Declaration = await declaration.ToModelAsync(repository);

            return result;
        }
    }
}
