using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities = Rubberduck.DataServer.Storage.Entities;
using Rubberduck.InternalApi.RPC.DataServer;
using Rubberduck.Server.LocalDb.Internal;
using StreamJsonRpc;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;

namespace Rubberduck.Server.LocalDb.Services
{
    public interface IDeclarationsProxy : IJsonRpcTarget
    {
        [RubberduckSP("saveAnnotations")]
        Task<IEnumerable<DeclarationAnnotation>> SaveAsync(IEnumerable<DeclarationAnnotation> annotations);

        [RubberduckSP("saveAttributes")]
        Task<IEnumerable<DeclarationAttribute>> SaveAsync(IEnumerable<DeclarationAttribute> attributes);

        [RubberduckSP("saveIdentifierReferences")]
        Task<IEnumerable<IdentifierReference>> SaveAsync(IEnumerable<IdentifierReference> identifierReferences);

        [RubberduckSP("saveProjects")]
        Task<IEnumerable<Project>> SaveAsync(IEnumerable<Project> projects);

        [RubberduckSP("saveModules")]
        Task<IEnumerable<Module>> SaveAsync(IEnumerable<Module> modules);

        [RubberduckSP("saveMembers")]
        Task<IEnumerable<Member>> SaveAsync(IEnumerable<Member> members);

        [RubberduckSP("saveParameters")]
        Task<IEnumerable<Parameter>> SaveAsync(IEnumerable<Parameter> parameters);

        [RubberduckSP("saveLocals")]
        Task<IEnumerable<Local>> SaveAsync(IEnumerable<Local> locals);
    }

    public class DeclarationsService : IDeclarationsProxy
    {
        private readonly IUnitOfWorkFactory _factory;

        public DeclarationsService(IUnitOfWorkFactory factory)
        {
            /* TODO inject some service layer and lighten up the RPC entry point layer */

            _factory = factory;
        }

        private Entities.Declaration FromModel(Project model)
        {
            return new Entities.Declaration
            {
                AsTypeDeclarationId = null,
                AsTypeName = null,
                ContextEndOffset = null,
                ContextStartOffset = null,
                DeclarationType = (long)model.DeclarationType,
                DocString = null,
                Id = model.DeclarationId,
                IdentifierName = model.IdentifierName,
                IdentifierEndOffset = null,
                IdentifierStartOffset = null,
                IsArray = 0,
                IsUserDefined = model.IsUserDefined ? 1 : 0,
                ParentDeclarationId = null,
                TypeHint = null,
            };
        }

        private Entities.Declaration FromModel(Module model)
        {
            return new Entities.Declaration
            {
                AsTypeDeclarationId = model.DeclarationType.HasFlag(InternalApi.Model.DeclarationType.ClassModule) ? (int?)model.DeclarationId : null,
                AsTypeName = model.DeclarationType.HasFlag(InternalApi.Model.DeclarationType.ClassModule) ? model.IdentifierName : null,
                ContextEndOffset = null,
                ContextStartOffset = null,
                DeclarationType = (long)model.DeclarationType,
                DocString = model.DocString,
                Id = model.Id,
                IdentifierName = model.IdentifierName,
                IdentifierEndOffset = null,
                IdentifierStartOffset = null,
                IsArray = 0,
                IsUserDefined = model.IsUserDefined ? 1 : 0,
                ParentDeclarationId = model.ProjectDeclarationId,
                TypeHint = null,
            };
        }

        private Entities.Declaration FromModel(Member model)
        {
            return new Entities.Declaration
            {
                AsTypeDeclarationId = model.AsTypeDeclarationId,
                AsTypeName = model.AsTypeName,
                ContextEndOffset = model.LocationInfo.ContextOffset.End,
                ContextStartOffset = model.LocationInfo.ContextOffset.Start,
                DeclarationType = (long)model.DeclarationType,
                DocString = model.DocString,
                Id = model.Id,
                IdentifierName = model.IdentifierName,
                IdentifierEndOffset = model.LocationInfo.IdentifierOffset.End,
                IdentifierStartOffset = model.LocationInfo.IdentifierOffset.Start,
                IsArray = model.IsArray ? 1 : 0,
                IsUserDefined = model.IsUserDefined ? 1 : 0,
                ParentDeclarationId = model.ModuleDeclarationId,
                TypeHint = model.TypeHint,
            };
        }

        private Entities.Declaration FromModel(Parameter model)
        {
            return new Entities.Declaration
            {
                AsTypeDeclarationId = model.AsTypeDeclarationId,
                AsTypeName = model.AsTypeName,
                ContextEndOffset = model.LocationInfo.ContextOffset.End,
                ContextStartOffset = model.LocationInfo.ContextOffset.Start,
                DeclarationType = (long)model.DeclarationType,
                DocString = model.DocString,
                Id = model.Id,
                IdentifierName = model.IdentifierName,
                IdentifierEndOffset = model.LocationInfo.IdentifierOffset.End,
                IdentifierStartOffset = model.LocationInfo.IdentifierOffset.Start,
                IsArray = model.IsArray ? 1 : 0,
                IsUserDefined = model.IsUserDefined ? 1 : 0,
                ParentDeclarationId = model.ModuleDeclarationId,
                TypeHint = model.TypeHint,
            };
        }

        private Entities.Declaration FromModel(Local model)
        {
            return new Entities.Declaration
            {
                AsTypeDeclarationId = model.AsTypeDeclarationId,
                AsTypeName = model.AsTypeName,
                ContextEndOffset = model.LocationInfo.ContextOffset.End,
                ContextStartOffset = model.LocationInfo.ContextOffset.Start,
                DeclarationType = (long)model.DeclarationType,
                DocString = model.DocString,
                Id = model.Id,
                IdentifierName = model.IdentifierName,
                IdentifierEndOffset = model.LocationInfo.IdentifierOffset.End,
                IdentifierStartOffset = model.LocationInfo.IdentifierOffset.Start,
                IsArray = model.IsArray ? 1 : 0,
                IsUserDefined = model.IsUserDefined ? 1 : 0,
                ParentDeclarationId = model.ModuleDeclarationId,
                TypeHint = model.TypeHint,
            };
        }

        private Entities.DeclarationAnnotation FromModel(DeclarationAnnotation model)
        {
            return new Entities.DeclarationAnnotation
            {
                AnnotationArgs = model.AnnotationArgs,
                AnnotationName = model.AnnotationName,
                ContextEndOffset = model.LocationInfo.ContextOffset.End,
                ContextStartOffset = model.LocationInfo.ContextOffset.Start,
                DeclarationId = model.DeclarationId,
                Id = model.Id,
                IdentifierEndOffset = model.LocationInfo.IdentifierOffset.End,
                IdentifierStartOffset = model.LocationInfo.IdentifierOffset.Start,
            };
        }

        private Entities.DeclarationAttribute FromModel(DeclarationAttribute model)
        {
            return new Entities.DeclarationAttribute
            {
                AttributeName = model.AttributeName,
                AttributeValues = model.AttributeValues,
                ContextEndOffset = model.LocationInfo.ContextOffset.End,
                ContextStartOffset = model.LocationInfo.ContextOffset.Start,
                DeclarationId = model.DeclarationId,
                Id = model.Id,
                IdentifierEndOffset = model.LocationInfo.IdentifierOffset.End,
                IdentifierStartOffset = model.LocationInfo.IdentifierOffset.Start,
            };
        }

        private Entities.IdentifierReference FromModel(IdentifierReference model)
        {
            return new Entities.IdentifierReference
            {
                ContextEndOffset = model.LocationInfo.ContextOffset.End,
                ContextStartOffset = model.LocationInfo.ContextOffset.Start,
                DefaultMemberRecursionDepth = model.DefaultMemberRecursionDepth,
                Id = model.Id,
                IdentifierEndOffset = model.LocationInfo.IdentifierOffset.End,
                IdentifierStartOffset = model.LocationInfo.IdentifierOffset.Start,
                IsArrayAccess = model.IsArrayAccess ? 1 : 0,
                IsAssignmentTarget = model.IsAssignmentTarget ? 1 : 0,
                IsExplicitCallStatement = model.IsExplicitCallStatement ? 1 : 0,
                IsExplicitLetAssignment = model.IsExplicitLetAssignment ? 1 : 0,
                IsIndexedDefaultMemberAccess = model.IsIndexedDefaultMemberAccess ? 1 : 0,
                IsInnerRecursiveDefaultMemberAccess = model.IsInnerRecursiveDefaultMemberAccess ? 1 : 0,
                IsNonIndexedDefaultMemberAccess = model.IsNonIndexedDefaultMemberAccess ? 1 : 0,
                IsProcedureCoercion = model.IsProcedureCoercion ? 1 : 0,
                IsReDim = model.IsReDim ? 1 : 0,
                IsSetAssignment = model.IsSetAssignment ? 1 : 0,
                MemberDeclarationId = model.MemberDeclarationId,
                ModuleDeclarationId = model.ModuleDeclarationId,
                ProjectDeclarationId = model.ProjectDeclarationId,
                QualifyingReferenceId = model.QualifyingReferenceId,
                ReferenceDeclarationId = model.ReferenceDeclarationId,
                TypeHint = model.TypeHint,
            };
        }

        /// <summary>
        /// Saves the specified declaration annotations to the database.
        /// </summary>
        /// <remarks>
        /// Items with ID=0 are considered new inserts.
        /// </remarks>
        /// <returns>The saved declaration annotations, including their server-provided ID.</returns>
        [JsonRpcMethod("saveAnnotations")]
        public async Task<IEnumerable<DeclarationAnnotation>> SaveAsync(IEnumerable<DeclarationAnnotation> annotations)
        {
            using (var uow = _factory.CreateNew())
            {
                var annotationsRepo = uow.GetRepository<Entities.DeclarationAnnotation>();

                var operations = annotations.Select(async annotation =>
                {
                    var entity = FromModel(annotation);
                    await annotationsRepo.SaveAsync(entity);

                    annotation.Id = entity.Id;

                }).ToList();

                await Task.WhenAll(operations).ContinueWith(t => uow.SaveChanges());
                return annotations;
            }
        }

        /// <summary>
        /// Saves the specified declaration attributes to the database.
        /// </summary>
        /// <remarks>
        /// Items with ID=0 are considered new inserts.
        /// </remarks>
        /// <returns>The saved declaration attributes, including their server-provided ID.</returns>
        [JsonRpcMethod("saveAttributes")]
        public async Task<IEnumerable<DeclarationAttribute>> SaveAsync(IEnumerable<DeclarationAttribute> attributes)
        {
            using (var uow = _factory.CreateNew())
            {
                var attributesRepo = uow.GetRepository<Entities.DeclarationAttribute>();

                var operations = attributes.Select(async attribute =>
                {
                    var entity = FromModel(attribute);
                    await attributesRepo.SaveAsync(entity);

                    attribute.Id = entity.Id;

                }).ToList();

                await Task.WhenAll(operations).ContinueWith(t => uow.SaveChanges());
                return attributes;
            }
        }

        /// <summary>
        /// Saves the specified identifier references to the database.
        /// </summary>
        /// <remarks>
        /// Items with ID=0 are considered new inserts.
        /// </remarks>
        /// <returns>The saved identifier references, including their server-provided ID.</returns>
        [JsonRpcMethod("saveIdentifierReferences")]
        public async Task<IEnumerable<IdentifierReference>> SaveAsync(IEnumerable<IdentifierReference> identifierReferences)
        {
            using (var uow = _factory.CreateNew())
            {
                var identifierReferencesRepo = uow.GetRepository<Entities.IdentifierReference>();

                var operations = identifierReferences.Select(async identifierReference =>
                {
                    var entity = FromModel(identifierReference);
                    await identifierReferencesRepo.SaveAsync(entity);

                    identifierReference.Id = entity.Id;

                }).ToList();

                await Task.WhenAll(operations).ContinueWith(t => uow.SaveChanges());
                return identifierReferences;
            }
        }

        /// <summary>
        /// Saves the specified declarations to the database.
        /// </summary>
        /// <remarks>
        /// Items with ID=0 are considered new inserts.
        /// </remarks>
        /// <returns>The saved declarations, including their server-provided ID.</returns>
        [JsonRpcMethod("saveProjects")]
        public async Task<IEnumerable<Project>> SaveAsync(IEnumerable<Project> projects)
        {
            using (var uow = _factory.CreateNew())
            {
                var declarationsRepo = uow.GetRepository<Entities.Declaration>();
                var projectsRepo = uow.GetRepository<Entities.Project>();

                var operations = projects.Select(async project =>
                {
                    var declarationEntity = FromModel(project);
                    await declarationsRepo.SaveAsync(declarationEntity);

                    var projectEntity = new Entities.Project
                    {
                        DeclarationId = declarationEntity.Id,
                        Guid = project.Guid?.ToString(),
                        Id = project.Id,
                        MajorVersion = project.MajorVersion,
                        MinorVersion = project.MinorVersion,
                        Path = project.Path,
                        VBProjectId = project.VBProjectId,
                    };
                    await projectsRepo.SaveAsync(projectEntity);

                    project.Id = projectEntity.Id;
                    project.DeclarationId = projectEntity.DeclarationId;

                }).ToList();

                await Task.WhenAll(operations).ContinueWith(t => uow.SaveChanges());
                return projects;
            }
        }

        /// <summary>
        /// Saves the specified declarations to the database.
        /// </summary>
        /// <remarks>
        /// Items with ID=0 are considered new inserts.
        /// </remarks>
        /// <returns>The saved declarations, including their server-provided ID.</returns>
        [JsonRpcMethod("saveModules")]
        public async Task<IEnumerable<Module>> SaveAsync(IEnumerable<Module> modules)
        {
            using (var uow = _factory.CreateNew())
            {
                var declarationsRepo = uow.GetRepository<Entities.Declaration>();
                var modulesRepo = uow.GetRepository<Entities.Module>();

                var operations = modules.Select(async module =>
                {
                    var declarationEntity = FromModel(module);
                    await declarationsRepo.SaveAsync(declarationEntity);

                    var moduleEntity = new Entities.Module
                    {
                        DeclarationId = declarationEntity.Id,
                        Folder = module.Folder,
                        Id = module.Id,
                    };
                    await modulesRepo.SaveAsync(moduleEntity);

                    module.Id = moduleEntity.Id;
                    module.DeclarationId = moduleEntity.DeclarationId;                    

                }).ToList();

                await Task.WhenAll(operations).ContinueWith(t => uow.SaveChanges());
                return modules;
            }
        }

        /// <summary>
        /// Saves the specified declarations to the database.
        /// </summary>
        /// <remarks>
        /// Items with ID=0 are considered new inserts.
        /// </remarks>
        /// <returns>The saved declarations, including their server-provided ID.</returns>
        [JsonRpcMethod("saveMembers")]
        public async Task<IEnumerable<Member>> SaveAsync(IEnumerable<Member> members)
        {
            using (var uow = _factory.CreateNew())
            {
                var declarationsRepo = uow.GetRepository<Entities.Declaration>();
                var membersRepo = uow.GetRepository<Entities.Member>();

                var operations = members.Select(async member =>
                {
                    var declarationEntity = FromModel(member);
                    await declarationsRepo.SaveAsync(declarationEntity);

                    var memberEntity = new Entities.Member
                    {
                        DeclarationId = declarationEntity.Id,
                        Accessibility = (int)member.Accessibility,
                        ImplementsDeclarationId = member.ImplementsDeclarationId,
                        IsAutoAssigned = member.IsAutoAssigned ? 1 : 0,
                        IsDimStmt = member.IsDimStmt ? 1 : 0,
                        IsWithEvents = member.IsWithEvents ? 1 : 0,
                        ValueExpression = member.ValueExpression,
                        Id = member.Id,
                    };
                    await membersRepo.SaveAsync(memberEntity);

                    member.Id = memberEntity.Id;
                    member.DeclarationId = memberEntity.DeclarationId;

                }).ToList();

                await Task.WhenAll(operations).ContinueWith(t => uow.SaveChanges());
                return members;
            }
        }

        /// <summary>
        /// Saves the specified declarations to the database.
        /// </summary>
        /// <remarks>
        /// Items with ID=0 are considered new inserts.
        /// </remarks>
        /// <returns>The saved declarations, including their server-provided ID.</returns>
        [JsonRpcMethod("saveParameters")]
        public async Task<IEnumerable<Parameter>> SaveAsync(IEnumerable<Parameter> parameters)
        {
            using (var uow = _factory.CreateNew())
            {
                var declarationsRepo = uow.GetRepository<Entities.Declaration>();
                var parametersRepo = uow.GetRepository<Entities.Parameter>();

                var operations = parameters.Select(async parameter =>
                {
                    var declarationEntity = FromModel(parameter);
                    await declarationsRepo.SaveAsync(declarationEntity);

                    var parameterEntity = new Entities.Parameter
                    {
                        DeclarationId = declarationEntity.Id,
                        DefaultValue = parameter.DefaultValue,
                        Id = parameter.Id,
                        IsByRef = parameter.IsByRef ? 1 : 0,
                        IsByVal = parameter.IsByVal ? 1 : 0,
                        IsModifierImplicit = parameter.IsModifierImplicit ? 1 : 0,
                        IsOptional = parameter.IsOptional ? 1 : 0,
                        IsParamArray = parameter.IsParamArray ? 1 : 0,
                        Position = parameter.OrdinalPosition,
                    };
                    await parametersRepo.SaveAsync(parameterEntity);

                    parameter.Id = parameterEntity.Id;
                    parameter.DeclarationId = parameterEntity.DeclarationId;

                }).ToList();

                await Task.WhenAll(operations).ContinueWith(t => uow.SaveChanges());
                return parameters;
            }
        }

        /// <summary>
        /// Saves the specified declarations to the database.
        /// </summary>
        /// <remarks>
        /// Items with ID=0 are considered new inserts.
        /// </remarks>
        /// <returns>The saved declarations, including their server-provided ID.</returns>
        [JsonRpcMethod("saveLocals")]
        public async Task<IEnumerable<Local>> SaveAsync(IEnumerable<Local> locals)
        {
            using (var uow = _factory.CreateNew())
            {
                var declarationsRepo = uow.GetRepository<Entities.Declaration>();
                var localsRepo = uow.GetRepository<Entities.Local>();

                var operations = locals.Select(async local =>
                {
                    var declarationEntity = FromModel(local);
                    await declarationsRepo.SaveAsync(declarationEntity);

                    var localEntity = new Entities.Local
                    {
                        DeclarationId = declarationEntity.Id,
                        Id = local.Id,
                        IsAutoAssigned = local.IsAutoAssigned ? 1 : 0,
                        IsImplicit = local.IsImplicit ? 1 : 0,
                        ValueExpression = local.ValueExpression,
                    };
                    await localsRepo.SaveAsync(localEntity);

                    local.Id = localEntity.Id;
                    local.DeclarationId = localEntity.DeclarationId;

                }).ToList();

                await Task.WhenAll(operations).ContinueWith(t => uow.SaveChanges());
                return locals;
            }
        }
    }
}
