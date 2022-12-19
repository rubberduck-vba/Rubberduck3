using Rubberduck.DataServices.Repositories;
using Rubberduck.InternalApi.Model;
using System.Threading.Tasks;

namespace Rubberduck.DataServices.Entities
{
    internal class Member : DbEntity
    {
        public int DeclarationId { get; set; }
        public int? ImplementsDeclarationId { get; set; }
        public int Accessibility { get; set; }
        public int IsAutoAssigned { get; set; }
        public int IsWithEvents { get; set; }
        public int IsDimStmt { get; set; }
        public string ValueExpression { get; set; }

        internal static Member FromModel(InternalApi.Model.RPC.Member model)
        {
            return new Member
            {
                Id = model.Id,
                DeclarationId = model.Declaration.Id,
                ImplementsDeclarationId = model.ImplementsDeclaration?.Id,
                Accessibility = (int)model.Accessibility,
                IsAutoAssigned = model.IsAutoAssigned ? 1 : 0,
                IsWithEvents = model.IsWithEvents ? 1 : 0,
                IsDimStmt = model.IsDimStmt ? 1 : 0,
                ValueExpression = model.ValueExpression,
            };
        }

        internal async Task<InternalApi.Model.RPC.Member> ToModelAsync(Repository<Declaration> repository)
        {
            var result = new InternalApi.Model.RPC.Member
            {
                Id = Id,
                Accessibility = (Accessibility)Accessibility,
                IsAutoAssigned = IsAutoAssigned != 0,
                IsWithEvents = IsWithEvents != 0,
                IsDimStmt = IsDimStmt != 0,
                ValueExpression = ValueExpression,
            };

            var declaration = await repository.GetByIdAsync(DeclarationId);
            result.Declaration = await declaration.ToModelAsync(repository);

            if (ImplementsDeclarationId.HasValue)
            {
                var implements = await repository.GetByIdAsync(ImplementsDeclarationId.Value);
                result.ImplementsDeclaration = await implements.ToModelAsync(repository);
            }

            return result;
        }
    }
}
