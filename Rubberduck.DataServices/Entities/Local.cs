using Rubberduck.DataServices.Repositories;
using System.Threading.Tasks;

namespace Rubberduck.DataServices.Entities
{
    internal class Local : DbEntity
    {
        public int DeclarationId { get; set; }
        public int IsAutoAssigned { get; set; }
        public string ValueExpression { get; set; }

        internal static Local FromModel(InternalApi.Model.RPC.Local model)
        {
            return new Local
            {
                Id = model.Id,
                IsAutoAssigned = model.IsAutoAssigned ? 1 : 0,
                ValueExpression = model.ValueExpression,
                DeclarationId = model.Declaration.Id
            };
        }

        internal async Task<InternalApi.Model.RPC.Local> ToModelAsync(Local entity, Repository<Declaration> repository)
        {
            var result = new InternalApi.Model.RPC.Local
            {
                Id = entity.Id,
                IsAutoAssigned = entity.IsAutoAssigned != 0,
                ValueExpression = entity.ValueExpression,
            };

            var declaration = await repository.GetByIdAsync(entity.DeclarationId);
            result.Declaration = await declaration.ToModelAsync(repository);

            return result;
        }
    }
}
