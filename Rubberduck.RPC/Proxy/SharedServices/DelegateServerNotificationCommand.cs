using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Proxy.SharedServices
{
    /// <summary>
    /// A command sent from a server proxy implementation to the actual server application,
    /// that injects delegates so that the command can be defined inline by its owner.
    /// </summary>
    /// <typeparam name="TParameter">The class type of the parameter passed to the command.</typeparam>
    public class DelegateServerNotificationCommand<TOptions> : ServerNotificationCommand<TOptions>
        where TOptions : class, new()
    {
        private readonly Func<Task> _execute;
        private readonly Func<Task<bool>> _canExecute;

        public DelegateServerNotificationCommand(string description, IServerLogger logger, GetServerOptionsAsync<TOptions> getConfiguration, GetServerStateInfoAsync getServerState)
            : base(logger, getConfiguration, getServerState)
        {
            Description = description;
        }

        public override string Description { get; }

        protected override IReadOnlyCollection<ServerStatus> ExpectedServerStates => new ServerStatus[0];

        public async override Task<bool> CanExecuteAsync() => await (_canExecute?.Invoke() ?? Task.FromResult(true));
        protected async override Task ExecuteInternalAsync() => await _execute.Invoke();
    }

    /// <summary>
    /// A command sent from a server proxy implementation to the actual server application,
    /// that injects delegates so that the command can be defined inline by its owner.
    /// </summary>
    /// <typeparam name="TParameter">The class type of the parameter passed to the command.</typeparam>
    public class DelegateServerNotificationCommand<TOptions, TParameter> : ServerNotificationCommand<TOptions, TParameter>
        where TOptions : class, new()
        where TParameter : class, new()
    {
        private readonly Func<TParameter, Task> _execute;
        private readonly Func<TParameter, Task<bool>> _canExecute;

        public DelegateServerNotificationCommand(string description, IServerLogger logger, GetServerOptionsAsync<TOptions> getConfiguration, GetServerStateInfoAsync getServerState) 
            : base(logger, getConfiguration, getServerState)
        {
            Description = description;
        }

        public override string Description { get; }

        protected override IReadOnlyCollection<ServerStatus> ExpectedServerStates => new ServerStatus[0];
        
        public async override Task<bool> CanExecuteAsync(TParameter parameter) => await (_canExecute?.Invoke(parameter) ?? Task.FromResult(true));
        protected async override Task ExecuteInternalAsync(TParameter parameter) => await _execute.Invoke(parameter);
    }
}