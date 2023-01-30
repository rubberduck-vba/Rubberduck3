using Rubberduck.RPC.Platform.Exceptions;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Proxy.SharedServices
{
    public abstract class ServerRequestCommand<TParameter, TResult, TOptions> : ServerCommandBase<TOptions>, IServerRequestCommand<TParameter, TResult>
        where TParameter : class, new()
        where TResult : class, new()
        where TOptions : class, new()
    {
        protected ServerRequestCommand(IServerLogger logger, GetServerOptions<TOptions> getConfiguration, GetServerStateInfo getServerState)
            : base(logger, getConfiguration, getServerState)
        {
        }

        public Func<TParameter, CancellationToken, Task<TResult>> ExecuteAction => (parameter, token) => ExecuteAsync(parameter, token);

        public Func<TParameter, CancellationToken, Task<bool>> CanExecuteFunc => (parameter, token) => CanExecuteAsync(parameter, token);

        public virtual async Task<bool> CanExecuteAsync(TParameter parameter, CancellationToken token)
        {
            var state = GetCurrentServerStateInfo().Status;
            var result = ExpectedServerStates.Contains(state);

            return await Task.FromResult(result);
        }

        /// <summary>
        /// The actual command implementation.
        /// </summary>
        /// <remarks>
        /// Server state is valid and exceptions are handled when this method is invoked.
        /// </remarks>
        protected abstract Task<TResult> ExecuteInternalAsync(TParameter parameter, CancellationToken token);

        public async Task<TResult> ExecuteAsync(TParameter parameter, CancellationToken token)
        {
            try
            {
                Logger.OnTrace($"Executing command '{GetType().Name}'");

                ThrowOnUnexpectedServerState();
                token.ThrowIfCancellationRequested();

                return await ExecuteInternalAsync(parameter, token);
            }
            catch (ApplicationException exception)
            {
                Logger.OnError(exception);
            }
            catch (Exception exception) when (!(exception is OperationCanceledException))
            {
                Logger.OnError(exception);
                throw;
            }

            return null;
        }

        /// <summary>
        /// The expected <c>ServerState</c> values when executing this command.
        /// </summary>
        /// <remarks>
        /// Executing the command despite the expected server state may throw an <c>InvalidStateException</c>.
        /// Unless overriden, expected server states only include <c>ServerState.Initialized</c>.
        /// </remarks>
        protected virtual IReadOnlyCollection<ServerStatus> ExpectedServerStates { get; } = new[] { ServerStatus.Initialized, };

        /// <summary>
        /// Throws an exception if the server is not in a valid state for this command.
        /// </summary>
        /// <exception cref="InvalidStateException"></exception>
        protected void ThrowOnUnexpectedServerState()
        {
            var state = GetCurrentServerStateInfo().Status;
            if (ExpectedServerStates.Any() && !ExpectedServerStates.Contains(state))
            {
                throw new InvalidStateException(GetType().Name, state, ExpectedServerStates.ToArray());
            }
        }

        public async Task<(bool, TResult)> TryExecuteAsync(TParameter parameter, CancellationToken token)
        {
            try
            {
                if (await CanExecuteAsync(parameter, token))
                {
                    var result = await ExecuteAsync(parameter, token);
                    return (true, result);
                }
            }
            catch (Exception exception) when (!(exception is OperationCanceledException))
            {
                Logger.OnError(exception);
            }

            return (false, null);
        }
    }

    public abstract class ServerRequestCommand<TResult, TOptions> : ServerCommandBase<TOptions>, IServerRequestCommand<TResult>
        where TResult : class, new()
        where TOptions : class, new()
    {
        protected ServerRequestCommand(IServerLogger logger, GetServerOptions<TOptions> getConfiguration, GetServerStateInfo getServerState)
            : base(logger, getConfiguration, getServerState)
        {
        }

        public Func<CancellationToken, Task<TResult>> ExecuteAction => token => ExecuteAsync(token);

        public Func<CancellationToken, Task<bool>> CanExecuteFunc => token => CanExecuteAsync(token);

        public virtual async Task<bool> CanExecuteAsync(CancellationToken token)
        {
            var state = GetCurrentServerStateInfo().Status;
            var result = ExpectedServerStates.Contains(state);

            return await Task.FromResult(result);
        }

        /// <summary>
        /// The actual command implementation.
        /// </summary>
        /// <remarks>
        /// Server state is valid and exceptions are handled when this method is invoked.
        /// </remarks>
        protected abstract Task<TResult> ExecuteInternalAsync(CancellationToken token);

        public async Task<TResult> ExecuteAsync(CancellationToken token)
        {
            try
            {
                Logger.OnTrace($"Executing command {GetType().Name}");

                ThrowOnUnexpectedServerState();
                token.ThrowIfCancellationRequested();

                return await ExecuteInternalAsync(token);
            }
            catch (ApplicationException exception)
            {
                Logger.OnError(exception);
            }
            catch (Exception exception) when (!(exception is OperationCanceledException))
            {
                Logger.OnError(exception);
                throw;
            }

            return null;
        }

        /// <summary>
        /// The expected <c>ServerState</c> values when executing this command.
        /// </summary>
        /// <remarks>
        /// Executing the command despite the expected server state may throw an <c>InvalidStateException</c>.
        /// Unless overriden, expected server states only include <c>ServerState.Initialized</c>.
        /// </remarks>
        protected virtual IReadOnlyCollection<ServerStatus> ExpectedServerStates { get; } = new[] { ServerStatus.Initialized, };

        /// <summary>
        /// Throws an exception if the server is not in a valid state for this command.
        /// </summary>
        /// <exception cref="InvalidStateException"></exception>
        protected void ThrowOnUnexpectedServerState()
        {
            var state = GetCurrentServerStateInfo().Status;
            if (ExpectedServerStates.Any() && !ExpectedServerStates.Contains(state))
            {
                throw new InvalidStateException(GetType().Name, state, ExpectedServerStates.ToArray());
            }
        }

        public async Task<(bool, TResult)> TryExecuteAsync(CancellationToken token)
        {
            try
            {
                if (await CanExecuteAsync(token))
                {
                    var result = await ExecuteAsync(token);
                    return (true, result);
                }
            }
            catch (Exception exception) when (!(exception is OperationCanceledException))
            {
                Logger.OnError(exception);
            }

            return (false, null);
        }
    }
}