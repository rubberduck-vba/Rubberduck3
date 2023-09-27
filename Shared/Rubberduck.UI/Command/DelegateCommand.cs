using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
//using NLog;

namespace Rubberduck.UI.Command
{
    [ComVisible(false)]
    public class DelegateCommand : CommandBase
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        public DelegateCommand(ILogger logger, Action<object> execute, Predicate<object> canExecute = null) 
            : base(logger)
        {
            _canExecute = canExecute;
            _execute = execute;

            AddToCanExecuteEvaluation(SpecialEvaluateCanExecute);
        }

        private bool SpecialEvaluateCanExecute(object parameter)
        {
            return _canExecute == null || _canExecute.Invoke(parameter);
        }

        protected async override Task OnExecuteAsync(object parameter)
        {
            await Task.Run(() => _execute.Invoke(parameter));
        }
    }
}
