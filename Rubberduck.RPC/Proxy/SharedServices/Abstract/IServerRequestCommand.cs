using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Proxy.SharedServices.Abstract
{
    /// <summary>
    /// Represents a server command executed in response to a request from a client.
    /// </summary>
    /// <typeparam name="TParameter">The class type of the parameter passed to the command.</typeparam>
    /// <typeparam name="TResult">The class type of the response.</typeparam>
    public interface IServerRequestCommand<TParameter, TResult>
        where TParameter : class, new()
        where TResult : class, new()
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
        Task<TResult> ExecuteAsync(TParameter parameter, CancellationToken token);

        /// <summary>
        /// Asynchrously executes the command if <c>CanExecuteAsync</c> returns <c>true</c>.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>A value tuple with <c>.success:true</c> and a valid <c>result</c> reference if the command was invoked, <c>(false, null)</c> otherwise.</returns>
        Task<(bool success, TResult result)> TryExecuteAsync(TParameter parameter, CancellationToken token);
    }

    /// <summary>
    /// Represents a parameterless server command executed in response to a request from a client.
    /// </summary>
    /// <typeparam name="TResult">The class type of the response.</typeparam>
    public interface IServerRequestCommand<TResult>
        where TResult : class, new()
    {
        /// <summary>
        /// Asynchronously evaluates whether the command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed, <c>false</c> otherwise.</returns>
        Task<bool> CanExecuteAsync(CancellationToken token);

        /// <summary>
        /// Asynchronously executes the command on the server.
        /// </summary>
        Task<TResult> ExecuteAsync(CancellationToken token);

        /// <summary>
        /// Asynchrously executes the command if <c>CanExecuteAsync</c> returns <c>true</c>.
        /// </summary>
        /// <returns>A value tuple with <c>.success:true</c> and a valid <c>result</c> reference if the command was invoked, <c>(false, null)</c> otherwise.</returns>
        Task<(bool success, TResult result)> TryExecuteAsync(CancellationToken token);
    }
}