using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rubberduck.DataServices.Repositories;
using Rubberduck.InternalApi.Model.RPC;
using Rubberduck.InternalApi.Model.DeclarationTree;
using System.Collections;

namespace Rubberduck.DataServices
{
    public interface IDeclarationTreeService
    {
        Task<DeclarationNode> GetModuleTreeAsync(IModule module);
    }

    public class DeclarationTreeService : IDeclarationTreeService
    {
        private readonly IUnitOfWorkFactory _factory;

        internal DeclarationTreeService(IUnitOfWorkFactory factory)
        {
            _factory = factory;
        }


        public async Task<DeclarationNode> GetModuleTreeAsync(IModule module)
        {
            DeclarationNode result;
            IEnumerable<DeclarationNode> members;
            using (var context = _factory.CreateNew())
            {
                var sql = "SELECT * FROM [Declarations] WHERE [Id]=@id";
                var declaration = await context.QuerySingleOrDefaultAsync<Entities.Declaration>(sql, new { id = module.DeclarationId });
                if (declaration != null)
                {
                    members = GetMembers(context, declaration);
                }
            }
            return result;
        }

        private async IEnumerable<DeclarationNode> GetMembers(IUnitOfWork context, Entities.Declaration module)
        {
            var sql = "SELECT * FROM [Members_v1] WHERE [ModuleDeclarationId]=@id";
            var members = await context.QueryAsync<Entities.Member>(sql, new { id = module.Id });

            foreach (var member in members)
            {
                var parameters = GetParameters(context, member);
                var locals = GetLocals(context, member);
            }
        }

        private async IEnumerable<DeclarationNode> GetParameters(IUnitOfWork context, Entities.Declaration member)
        {
            var sql = "SELECT * FROM [Parameters_v1] WHERE [MemberDeclarationId]=@id";
            var parameters = await context.QueryAsync<Entities.Parameter>(sql, new { id = member.Id });


            return parameters.Select(parameter => 
                new ParameterDeclarationNode(parameter, declaration, member, asTypeDeclaration));
        }
    }

    internal class DeclarationFinderService
    {
        private readonly IUnitOfWorkFactory _factory;

        public DeclarationFinderService(IUnitOfWorkFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Gets the event handler procedures for a given event declaration.
        /// </summary>
        /// <param name="eventDeclaration">The <c>Event</c> declaration to find all handler procedures for.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Member>> FindEventHandlersAsync(Declaration eventDeclaration) 
        {
            var eventProviderDeclarationId = eventDeclaration.ParentDeclarationId;
            using (var context = _factory.CreateNew() as UnitOfWork)
            {
                var sql = @"
        WITH withEventsMembers AS (
        SELECT m.*
        FROM [Members] m
        INNER JOIN [Declarations] d ON m.[DeclarationId] = d.[Id]
        WHERE m.[IsWithEvents] = 1 AND d.[AsTypeDeclarationId] = @eventProviderDeclarationId
        )
        SELECT m.[Id],m.[DeclarationId],m.[ImplementsDeclarationId],m.[Accessibility],m.[IsAutoAssigned],m.[IsWithEvents],m.[IsDimStmt],m.[ValueExpression]
        FROM [Members] m
        INNER JOIN withEventsMembers e ON m.[ImplementsDeclarationId] = e.[Id];
        ";
                var members = await context.QueryAsync<Member>(sql, eventProviderDeclarationId);

                var allDeclarationIds = new HashSet<int>(
                    members.Select(m => m.DeclarationId).Concat(members.Select(m => m.ImplementsDeclarationId.Value)));

                var declarations = (await context.Declarations.GetByIdAsync(allDeclarationIds)).ToDictionary(e => e.Id);

                foreach (var member in members)
                {
                    //member.Declaration = declarations[member.DeclarationId];
                    //member.ImplementsDeclaration = declarations[member.ImplementsDeclarationId.Value];
                }

                return members;
            }
        }
    }

    internal class DeclarationModelService
    {
        private readonly IUnitOfWorkFactory _factory;

        public DeclarationModelService(IUnitOfWorkFactory factory)
        {
            _factory = factory;
        }

        private DeclarationRepository Declarations(IUnitOfWork context) 
            => (DeclarationRepository)context.GetRepository<Entities.Declaration>();
        private IdentifierReferenceRepository IdentifierReferences(IUnitOfWork context)
            => (IdentifierReferenceRepository)context.GetRepository<Entities.IdentifierReference>();

        private ProjectRepository Projects(IUnitOfWork context)
            => (ProjectRepository)context.GetRepository<Entities.Project>();
        private ModuleRepository Modules(IUnitOfWork context)
            => (ModuleRepository)context.GetRepository<Entities.Module>();
        private MemberRepository Members(IUnitOfWork context)
            => (MemberRepository)context.GetRepository<Entities.Member>();
        private ParameterRepository Parameters(IUnitOfWork context)
            => (ParameterRepository)context.GetRepository<Entities.Parameter>();
        private LocalRepository Locals(IUnitOfWork context)
            => (LocalRepository)context.GetRepository<Entities.Local>();

        public async Task SaveAsync(IProject project)
        {
            using (var context = _factory.CreateNew())
            {
                await SaveAsync(project, context);
                context.SaveChanges();
            }
        }

        public async Task SaveAsync(IProject project, IUnitOfWork context)
        {
            var declarations = Declarations(context);
            //await declarations.SaveAsync(Entities.Declaration.FromModel(project.Declaration));

            var projects = Projects(context);
            //await projects.SaveAsync(Entities.Project.FromModel(project));
        }

        public async Task SaveAsync(IModule module)
        {
            using (var context = _factory.CreateNew())
            {
                await SaveAsync(module, context);
                context.SaveChanges();
            }
        }

        public async Task SaveAsync(IModule module, IUnitOfWork context)
        {
            var declarations = Declarations(context);
            //await declarations.SaveAsync(module);

            var modules = Modules(context);
            //await modules.SaveAsync(Entities.Module.FromModel(module));
        }

        public async Task SaveAsync(Member member)
        {
            using (var context = _factory.CreateNew())
            {
                await SaveAsync(member, context);
                context.SaveChanges();
            }
        }

        public async Task SaveAsync(Member member, IUnitOfWork context)
        {
            var declarations = Declarations(context);
            //await declarations.SaveAsync(Entities.Declaration.FromModel(member.Declaration));

            var members = Members(context);
            //await members.SaveAsync(Entities.Member.FromModel(member));
        }

        public async Task SaveAsync(Parameter parameter)
        {
            using (var context = _factory.CreateNew())
            {
                await SaveAsync(parameter, context);
                context.SaveChanges();
            }
        }

        public async Task SaveAsync(Parameter parameter, IUnitOfWork context)
        {
            var declarations = Declarations(context);
            //await declarations.SaveAsync(Entities.Declaration.FromModel(parameter.Declaration));

            var parameters = Parameters(context);
            //await parameters.SaveAsync(Entities.Parameter.FromModel(parameter));
        }

        public async Task SaveAsync(Local local)
        {
            using (var context = _factory.CreateNew())
            {
                await SaveAsync(local, context);
                context.SaveChanges();
            }
        }

        public async Task SaveAsync(Local local, IUnitOfWork context)
        {
            var declarations = Declarations(context);
            //await declarations.SaveAsync(local.Declaration);

            var locals = Locals(context);
            //await locals.SaveAsync(local);
        }

        public async Task SaveAsync(IEnumerable<Declaration> declarations)
        {
            using (var context = _factory.CreateNew())
            {
                await Task.WhenAll(declarations.Select(declaration => SaveAsync(declaration, context)));
                context.SaveChanges();
            }
        }

        public async Task SaveAsync(Declaration declaration)
        {
            using (var context = _factory.CreateNew())
            {
                await SaveAsync(declaration, context);
                context.SaveChanges();
            }
        }

        private async Task SaveAsync(Declaration declaration, IUnitOfWork context)
        {
            var repository = Declarations(context);
            //await repository.SaveAsync(declaration);
        }

        public async Task SaveAsync(IEnumerable<IdentifierReference> identifierReferences)
        {
            using (var context = _factory.CreateNew())
            {
                var repository = IdentifierReferences(context);
                foreach (var reference in identifierReferences)
                {
                    //await IdentifierReferences(context).SaveAsync(reference);
                }
                context.SaveChanges();
            }
        }
    }
}
