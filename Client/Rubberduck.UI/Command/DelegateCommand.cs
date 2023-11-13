using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Rubberduck.UI.Services;

namespace Rubberduck.UI.Command
{
    [ComVisible(false)]
    public class DelegateCommand : CommandBase
    {
        private readonly Predicate<object?>? _canExecute;
        private readonly Action<object?> _execute;

        public DelegateCommand(ServiceHelper service, Action<object?> execute, Predicate<object?>? canExecute = null) 
            : base(service)
        {
            _canExecute = canExecute;
            _execute = execute;

            AddToCanExecuteEvaluation(SpecialEvaluateCanExecute);
        }

        private bool SpecialEvaluateCanExecute(object? parameter) => _canExecute is null || _canExecute.Invoke(parameter);

        protected async override Task OnExecuteAsync(object? parameter) => await Task.Run(() => _execute.Invoke(parameter));
    }
}
