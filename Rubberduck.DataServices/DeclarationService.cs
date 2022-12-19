using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rubberduck.DataServices.Repositories;
using Rubberduck.InternalApi.Model.RPC;

namespace Rubberduck.DataServices
{
    internal class DeclarationService
    {
        private readonly IUnitOfWorkFactory _factory;

        public DeclarationService(IUnitOfWorkFactory factory)
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

        private async Task SaveAsync(Repository<Entities.Declaration> repository, Declaration declaration)
        {
            if (declaration.IsMarkedForDeletion)
            {
                await repository.DeleteAsync(declaration.Id);
            }
            else if (declaration.IsMarkedForUpdate || declaration.Id == default)
            {
                await repository.SaveAsync(Entities.Declaration.FromModel(declaration));
            }
        }

        public async Task SaveAsync(Project project)
        {
            using (var context = _factory.CreateNew())
            {
                var declarations = Declarations(context);
                await declarations.SaveAsync(Entities.Declaration.FromModel(project.Declaration));

                var projects = Projects(context);
                await projects.SaveAsync(Entities.Project.FromModel(project));

                context.SaveChanges();
            }
        }

        public async Task SaveAsync(Module module)
        {
            using (var context = _factory.CreateNew())
            {
                var declarations = Declarations(context);
                await declarations.SaveAsync(Entities.Declaration.FromModel(module.Declaration));

                var modules = Modules(context);
                await modules.SaveAsync(Entities.Module.FromModel(module));

                context.SaveChanges();
            }
        }

        public async Task SaveAsync(Member member)
        {
            using (var context = _factory.CreateNew())
            {
                var declarations = Declarations(context);
                await declarations.SaveAsync(Entities.Declaration.FromModel(member.Declaration));

                var members = Members(context);
                await members.SaveAsync(Entities.Member.FromModel(member));

                context.SaveChanges();
            }
        }

        public async Task SaveAsync(Parameter parameter)
        {
            using (var context = _factory.CreateNew())
            {
                var declarations = Declarations(context);
                await declarations.SaveAsync(Entities.Declaration.FromModel(parameter.Declaration));

                var parameters = Parameters(context);
                await parameters.SaveAsync(Entities.Parameter.FromModel(parameter));

                context.SaveChanges();
            }
        }

        public async Task SaveAsync(Local local)
        {
            using (var context = _factory.CreateNew())
            {
                var declarations = Declarations(context);
                await declarations.SaveAsync(Entities.Declaration.FromModel(local.Declaration));

                var locals = Locals(context);
                await locals.SaveAsync(Entities.Local.FromModel(local));

                context.SaveChanges();
            }
        }

        public async Task SaveAsync(IEnumerable<Declaration> declarations)
        {
            using (var context = _factory.CreateNew())
            {
                var repository = Declarations(context);
                await Task.WhenAll(declarations.Select(declaration => SaveAsync(repository, declaration)));

                context.SaveChanges();
            }
        }

        public async Task SaveAsync(IEnumerable<IdentifierReference> identifierReferences)
        {
            using (var context = _factory.CreateNew())
            {
                var repository = IdentifierReferences(context);
                foreach (var reference in identifierReferences)
                {
                    if (reference.IsMarkedForDeletion)
                    {
                        await repository.DeleteAsync(reference.Id);
                    }
                    else if (reference.IsMarkedForUpdate || reference.Id == default)
                    {
                        await IdentifierReferences(context).SaveAsync(Entities.IdentifierReference.FromModel(reference));
                    }
                }
                context.SaveChanges();
            }
        }
    }
}
