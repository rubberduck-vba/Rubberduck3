using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Proxy.SharedServices.Abstract
{
    /// <summary>
    /// Represents a parameterized server command executed in response to a notification from a client.
    /// </summary>
    /// <typeparam name="TParameter">The class type of the parameter passed to the command.</typeparam>
    public interface IServerNotificationCommand<TParameter>
        where TParameter : class, new()
    {
        /// <summary>
        /// Asynchronously evaluates whether the command can be executed.
        /// </summary>
        /// <param name="parameter">The parameter that would be passed to the <c>ExecuteAsync</c> method to execute the command.</param>
        /// <returns><c>true</c> if the command can be executed, <c>false</c> otherwise.</returns>
        Task<bool> CanExecuteAsync(TParameter parameter, CancellationToken token);
        /// <summary>
        /// Asynchronously executes the command on the server.
        /// </summary>
        /// <param name="parameter">A parameter containing information useful for executing this command.</param>
        Task ExecuteAsync(TParameter parameter, CancellationToken token);

        /// <summary>
        /// Asynchrously executes the command if <c>CanExecuteAsync</c> returns <c>true</c>.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns><c>true</c> if the command was invoked, <c>false</c> otherwise.</returns>
        Task<bool> TryExecuteAsync(TParameter parameter, CancellationToken token);
    }

    /// <summary>
    /// Represents a parameterless server command executed in response to a notification from a client.
    /// </summary>
    public interface IServerNotificationCommand
    {
        /// <summary>
        /// Asynchronously evaluates whether the command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed, <c>false</c> otherwise.</returns>
        Task<bool> CanExecuteAsync(CancellationToken token);
        /// <summary>
        /// Asynchronously executes the command on the server.
        /// </summary>
        Task ExecuteAsync(CancellationToken token);

        /// <summary>
        /// Asynchrously executes the command if <c>CanExecuteAsync</c> returns <c>true</c>.
        /// </summary>
        /// <returns><c>true</c> if the command was invoked, <c>false</c> otherwise.</returns>
        Task<bool> TryExecuteAsync(CancellationToken token);
    }
}