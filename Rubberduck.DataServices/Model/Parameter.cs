using Rubberduck.DataServices.Repositories;
using System.Threading.Tasks;

namespace Rubberduck.DataServices.Model
{
    public class Parameter
    {
        public int Id { get; set; }

        public Declaration Declaration { get; set; }
        public int Position { get; set; }
        public bool IsParamArray { get; set; }
        public bool IsOptional { get; set; }
        public bool IsByRef { get; set; }
        public bool IsByVal { get; set; }
        public bool IsModifierImplicit { get; set; }
        public string DefaultValue { get; set; }

        internal Entities.Parameter ToEntity()
        {
            return new Entities.Parameter
            {
                Id = Id,
                DeclarationId = Declaration.Id,
                Position = Position,
                IsParamArray = IsParamArray ? 1 : 0,
                IsOptional = IsOptional ? 1 : 0,
                IsByRef = IsByRef ? 1 : 0,
                IsByVal = IsByVal ? 1 : 0,
                IsModifierImplicit = IsModifierImplicit ? 1 : 0,
                DefaultValue = DefaultValue,
            };
        }

        internal async Task<Parameter> FromEntityAsync(Entities.Parameter entity, Repository<Entities.Declaration> repository)
        {
            var result = new Parameter
            {
                Id = entity.Id,
                Position = entity.Position,
                IsParamArray = entity.IsParamArray != 0,
                IsOptional = entity.IsOptional != 0,
                IsByRef = entity.IsByRef != 0,
                IsByVal = entity.IsByVal != 0,
                IsModifierImplicit = entity.IsModifierImplicit != 0,
                DefaultValue = DefaultValue,
            };

            var declaration = await repository.GetByIdAsync(entity.DeclarationId);
            result.Declaration = await Declaration.FromEntityAsync(declaration, repository);

            return result;
        }
    }
}
