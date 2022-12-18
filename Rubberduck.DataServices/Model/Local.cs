using Rubberduck.DataServices.Repositories;
using System.Threading.Tasks;

namespace Rubberduck.DataServices.Model
{
    internal class Local
    {
        public int Id { get; set; }

        public Declaration Declaration { get; set; }
        public bool IsAutoAssigned { get; set; }
        public string ValueExpression { get; set; }

        internal Entities.Local ToEntity()
        {
            return new Entities.Local
            {
                Id = Id,
                IsAutoAssigned = IsAutoAssigned ? 1 : 0,
                ValueExpression = ValueExpression,
                DeclarationId = Declaration.Id
            };
        }

        internal async Task<Local> FromEntityAsync(Entities.Local entity, Repository<Entities.Declaration> repository)
        {
            var result = new Local
            {
                Id = entity.Id,
                IsAutoAssigned = entity.IsAutoAssigned != 0,
                ValueExpression = entity.ValueExpression,
            };

            var declaration =await repository.GetByIdAsync(entity.DeclarationId);
            result.Declaration = await Declaration.FromEntityAsync(declaration, repository);

            return result;
        }
    }
}
