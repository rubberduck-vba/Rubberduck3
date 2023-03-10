using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using Rubberduck.DatabaseServer.Internal.Abstract;
using Rubberduck.InternalApi.Common;
using Rubberduck.ServerPlatform.Model;
using Rubberduck.ServerPlatform.Model.Entities;
using Rubberduck.ServerPlatform.RPC;

namespace Rubberduck.DatabaseServer.RPC.Save
{
    [Method(JsonRpcMethods.DatabaseServer.SaveMember, Direction.ClientToServer)]
    public class SaveMemberNotificationHandler : JsonRpcRequestHandler<SaveParam<Member>, SaveResult>
    {
        private readonly IUnitOfWorkFactory _factory;

        public SaveMemberNotificationHandler(ILogger<SaveMemberNotificationHandler> logger, IUnitOfWorkFactory factory)
            : base(logger)
        {
            _factory = factory;
        }

        public override Task<SaveResult> Handle(SaveParam<Member> request, CancellationToken cancellationToken)
        {
            return base.Handle(request, cancellationToken);
        }

        protected async override Task<SaveResult> HandleAsync(SaveParam<Member> request)
        {
            var elapsed = await TimedAction.RunAsync(SaveEntitiesAsync(request.Entities));
            Logger.LogTrace($"[PERF] {GetType().Name}.SaveEntitiesAsync completed in {elapsed.TotalMilliseconds:N0} ms.");
            return SaveResult.Success;
        }

        private async Task<SaveResult> SaveEntitiesAsync(IEnumerable<Member> entities)
        {
            using (var uow = _factory.CreateNew())
            {
                var repo = uow.GetRepository<Member>();
                foreach (var entity in entities)
                {
                    await repo.SaveAsync(entity);
                }

                uow.SaveChanges();
            }

            return SaveResult.Success;
        }
    }
}
