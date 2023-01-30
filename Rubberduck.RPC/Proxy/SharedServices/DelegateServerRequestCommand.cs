using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Proxy.SharedServices
{

    /// <summary>
    /// A request command sent from a server proxy implementation to the actual server application,
    /// that injects delegates so that the command can be defined inline by its owner.
    /// </summary>
    /// <typeparam name="TParameter">The class type of the parameter passed to the command.</typeparam>
    public class DelegateServerRequestCommand<TParameter, TResult, TOptions> : ServerRequestCommand<TParameter, TResult, TOptions>
        where TParameter : class, new()
        where TResult : class, new()
        where TOptions : class, new()
    {
        private readonly Func<TParameter, CancellationToken, Task<TResult>> _execute;
        private readonly Func<TParameter, CancellationToken, Task<bool>> _canExecute;

        public DelegateServerRequestCommand(string name, string description, IServerLogger logger, GetServerOptions<TOptions> getConfiguration, GetServerStateInfo getServerState,
            Func<TParameter, CancellationToken, Task<TResult>> executeDelegate, 
            Func<TParameter, CancellationToken, Task<bool>> canExecuteDelegate = null)
            : base(logger, getConfiguration, getServerState)
        {
            _execute = executeDelegate;
            _canExecute = canExecuteDelegate;

            Name = name;
            Description = description;
        }

        public override string Name { get; }
        public override string Description { get; }

        public async override Task<bool> CanExecuteAsync(TParameter parameter, CancellationToken token)
        {
            if (_canExecute is null) return true;
            return await _canExecute.Invoke(parameter, token);
        }

        protected async override Task<TResult> ExecuteInternalAsync(TParameter parameter, CancellationToken token)
        {
            return await _execute.Invoke(parameter, token);
        }
    }

    /// <summary>
    /// A parameterless request command sent from a server proxy implementation to the actual server application,
    /// that injects delegates so that the command can be defined inline by its owner.
    /// </summary>
    public class DelegateServerRequestCommand<TResult, TOptions> : ServerRequestCommand<TResult, TOptions>
        where TResult : class, new()
        where TOptions : class, new()
    {
        private readonly Func<CancellationToken, Task<TResult>> _execute;
        private readonly Func<CancellationToken, Task<bool>> _canExecute;

        public DelegateServerRequestCommand(string name, string description, IServerLogger logger, GetServerOptions<TOptions> getConfiguration, GetServerStateInfo getServerState,
            Func<CancellationToken, Task<TResult>> executeDelegate, Func<CancellationToken, Task<bool>> canExecuteDelegate = null)
            : base(logger, getConfiguration, getServerState)
        {
            _execute = executeDelegate;
            _canExecute = canExecuteDelegate;

            Name = name;
            Description = description;
        }

        public override string Name { get; }
        public override string Description { get; }

        public async override Task<bool> CanExecuteAsync(CancellationToken token)
        {
            if (_canExecute is null) return true;
            return await _canExecute.Invoke(token);
        }

        protected async override Task<TResult> ExecuteInternalAsync(CancellationToken token)
        {
            return await _execute.Invoke(token);
        }
    }
}