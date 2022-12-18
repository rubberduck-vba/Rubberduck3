using Rubberduck.DataServices.Repositories;
using Rubberduck.InternalApi.Model;
using System.Threading.Tasks;

namespace Rubberduck.DataServices.Model
{
    public class Member
    {
        public int Id { get; set; }

        public Declaration Declaration { get; set; }
        public Declaration ImplementsDeclaration { get; set; }
        public Accessibility Accessibility { get; set; }
        public bool IsAutoAssigned { get; set; }
        public bool IsWithEvents { get; set; }
        public bool IsDimStmt { get; set; }
        public string ValueExpression { get; set; }

        internal Entities.Member ToEntity()
        {
            return new Entities.Member
            {
                Id = Id,
                DeclarationId = Declaration.Id,
                ImplementsDeclarationId = ImplementsDeclaration?.Id,
                Accessibility = (int)Accessibility,
                IsAutoAssigned = IsAutoAssigned ? 1 : 0,
                IsWithEvents = IsWithEvents ? 1 : 0,
                IsDimStmt = IsDimStmt ? 1 : 0,
                ValueExpression = ValueExpression,
            };
        }

        internal async Task<Member> FromEntityAsync(Entities.Member entity, Repository<Entities.Declaration> repository)
        {
            var result = new Member
            {
                Id = entity.Id,
                Accessibility = (Accessibility)entity.Accessibility,
                IsAutoAssigned = entity.IsAutoAssigned != 0,
                IsWithEvents = entity.IsWithEvents != 0,
                IsDimStmt = entity.IsDimStmt != 0,
                ValueExpression = entity.ValueExpression,
            };

            var declaration = await repository.GetByIdAsync(entity.DeclarationId);
            result.Declaration = await Declaration.FromEntityAsync(declaration, repository);

            if (entity.ImplementsDeclarationId.HasValue)
            {
                var implements = await repository.GetByIdAsync(entity.ImplementsDeclarationId.Value);
                result.ImplementsDeclaration = await Declaration.FromEntityAsync(implements, repository);
            }

            return result;
        }
    }
}
