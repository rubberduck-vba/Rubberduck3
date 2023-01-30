using System;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Proxy.SharedServices
{
/*
    /// <summary>
    /// A command sent from a server proxy implementation to the actual server application,
    /// that injects delegates so that the command can be defined inline by its owner.
    /// </summary>
    /// <typeparam name="TParameter">The class type of the parameter passed to the command.</typeparam>
    public class DelegateServerNotificationCommand<TParameter> : ServerNotificationCommand<TParameter>
        where TParameter : class
    {
        private readonly Action<TParameter> _execute;
        private readonly Func<TParameter, bool> _canExecute;

        public DelegateServerNotificationCommand(Action<TParameter> executeDelegate, Func<TParameter, bool> canExecuteDelegate = null)
        {
            _execute = executeDelegate;
            _canExecute = canExecuteDelegate;
        }

        protected override ServerState[] ExpectedServerStates => new ServerState[] { };

        public async override Task<bool> CanExecuteAsync(TParameter parameter) => await Task.Run(() => _canExecute?.Invoke(parameter) ?? true);
        public async override Task ExecuteAsync(TParameter parameter) => await Task.Run(() => _execute.Invoke(parameter));
    }
*/
}