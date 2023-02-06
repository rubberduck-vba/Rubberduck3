using System;
using System.Threading.Tasks;

namespace Rubberduck.UI.Command
{
    public class AsyncDelegateCommand : CommandBase
    {
        private readonly Func<object, Task> _execute;
        private readonly Func<object, Task<bool>> _canExecute;

        public AsyncDelegateCommand(Func<object, Task> execute, Func<object, Task<bool>> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute ?? ((o) => Task.FromResult(true));
        }

        private bool SpecialEvaluateCanExecute(object parameter)
        {
            return _canExecute == null || Task.Run(async () => await _canExecute.Invoke(parameter)).Result;
        }

        protected override void OnExecute(object parameter)
        {
            Task.Run(async () => await _execute.Invoke(parameter));
        }
    }
}
