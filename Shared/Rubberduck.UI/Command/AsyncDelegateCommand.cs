﻿using System;
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

        //private bool SpecialEvaluateCanExecute(object parameter)
        //{
        //    return _canExecute is null 
        //        || _canExecute.Invoke(parameter).ConfigureAwait(false).GetAwaiter().GetResult();
        //}

        protected async override Task OnExecuteAsync(object parameter)
        {
            await _execute.Invoke(parameter);
        }
    }
}
