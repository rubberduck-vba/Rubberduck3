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
    /// <summary>
    /// Represents a parameterized server command executed in response to a notification from a client.
    /// </summary>
    /// <typeparam name="TParameter">The class type of the parameter passed to the command.</typeparam>
    public abstract class ServerNotificationCommand<TOptions, TParameter> : ServerCommandBase<TOptions>, IServerNotificationCommand<TParameter>
        where TParameter : class, new()
        where TOptions : class, new()
    {
        protected ServerNotificationCommand(IServerLogger logger, GetServerOptions<TOptions> getConfiguration, GetServerStateInfo getServerState)
            : base(logger, getConfiguration, getServerState)
        {
        }

        public Func<TParameter, Task> ExecuteAction => parameter => ExecuteAsync(parameter);

        public Func<TParameter, Task<bool>> CanExecuteFunc => parameter => CanExecuteAsync(parameter);

        /// <summary>
        /// Writes an error-level exception to the console.
        /// </summary>

        public virtual async Task<bool> CanExecuteAsync(TParameter parameter)
        {
            var state = GetCurrentServerStateInfo().Status;
            var result = ExpectedServerStates.Contains(state);

            return await Task.FromResult(result);
        }

        /// <summary>
        /// The actual command implementation.
        /// </summary>
        /// <remarks>
        /// Server state is valid and exceptions are handled.
        /// </remarks>
        protected abstract Task ExecuteInternalAsync(TParameter parameter);

        public async Task ExecuteAsync(TParameter parameter)
        {
            try
            {
                ThrowOnUnexpectedServerState();

                await ExecuteInternalAsync(parameter);
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

        public async Task<bool> TryExecuteAsync(TParameter parameter)
        {
            try
            {
                if (await CanExecuteAsync(parameter))
                {
                    await ExecuteAsync(parameter);
                    return true;
                }
            }
            catch (Exception exception) when (!(exception is OperationCanceledException))
            {
                Logger.OnError(exception);
            }

            return false;
        }
    }

    /// <summary>
    /// Represents a parameterless server command executed in response to a notification from a client.
    /// </summary>
    /// <typeparam name="TParameter">The class type of the parameter passed to the command.</typeparam>
    public abstract class ServerNotificationCommand<TOptions> : ServerCommandBase<TOptions>, IServerNotificationCommand
        where TOptions : class, new()
    {
        protected ServerNotificationCommand(IServerLogger logger, GetServerOptions<TOptions> getConfiguration, GetServerStateInfo getServerState)
            : base(logger, getConfiguration, getServerState)
        {
        }

        public Func<Task> ExecuteAction => () => ExecuteAsync();

        public Func<Task<bool>> CanExecuteFunc => () => CanExecuteAsync();

        public virtual async Task<bool> CanExecuteAsync()
        {
            var state = GetCurrentServerStateInfo().Status;
            var result = ExpectedServerStates.Contains(state);

            return await Task.FromResult(result);
        }

        /// <summary>
        /// The actual command implementation.
        /// </summary>
        /// <remarks>
        /// Server state is valid and exceptions are handled.
        /// </remarks>
        protected abstract Task ExecuteInternalAsync();

        public async Task ExecuteAsync()
        {
            try
            {
                ThrowOnUnexpectedServerState();
                await ExecuteInternalAsync();
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

        public async Task<bool> TryExecuteAsync()
        {
            try
            {
                if (await CanExecuteAsync())
                {
                    await ExecuteAsync();
                    return true;
                }
            }
            catch (Exception exception) when (!(exception is OperationCanceledException))
            {
                Logger.OnError(exception);
            }

            return false;
        }
    }
}