using Rubberduck.DataServices.Repositories;
using System.Threading.Tasks;

namespace Rubberduck.DataServices.Entities
{
    internal class Parameter : DbEntity
    {
        public int DeclarationId { get; set; }
        public int Position { get; set; }
        public int IsParamArray { get; set; }
        public int IsOptional { get; set; }
        public int IsByRef { get; set; }
        public int IsByVal { get; set; }
        public int IsModifierImplicit { get; set; }
        public string DefaultValue { get; set; }

        internal static Parameter FromModel(InternalApi.Model.RPC.Parameter model)
        {
            return new Parameter
            {
                Id = model.Id,
                DeclarationId = model.Declaration.Id,
                Position = model.Position,
                IsParamArray = model.IsParamArray ? 1 : 0,
                IsOptional = model.IsOptional ? 1 : 0,
                IsByRef = model.IsByRef ? 1 : 0,
                IsByVal = model.IsByVal ? 1 : 0,
                IsModifierImplicit = model.IsModifierImplicit ? 1 : 0,
                DefaultValue = model.DefaultValue,
            };
        }

        internal async Task<InternalApi.Model.RPC.Parameter> ToModelAsync(Parameter entity, Repository<Declaration> repository)
        {
            var result = new InternalApi.Model.RPC.Parameter
            {
                Id = entity.Id,
                Position = entity.Position,
                IsParamArray = entity.IsParamArray != 0,
                IsOptional = entity.IsOptional != 0,
                IsByRef = entity.IsByRef != 0,
                IsByVal = entity.IsByVal != 0,
                IsModifierImplicit = entity.IsModifierImplicit != 0,
                DefaultValue = entity.DefaultValue,
            };

            var declaration = await repository.GetByIdAsync(entity.DeclarationId);
            result.Declaration = await declaration.ToModelAsync(repository);

            return result;
        }
    }
}
