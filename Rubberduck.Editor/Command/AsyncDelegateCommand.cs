using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using System;
using System.Threading.Tasks;

namespace Rubberduck.Editor.Command
{
    public class AsyncDelegateCommand : CommandBase
    {
        private readonly Func<object?, Task> _execute;
        private readonly Func<object?, Task<bool>>? _canExecute;

        public AsyncDelegateCommand(ILogger logger, ISettingsProvider<RubberduckSettings> settings, Func<object?, Task> execute, Func<object?, Task<bool>>? canExecute = null)
            : base(logger, settings)
        {
            _execute = execute;
            _canExecute = canExecute ?? ((o) => Task.FromResult(true));
            AddToCanExecuteEvaluation(SpecialEvaluateCanExecute);
        }

        private bool SpecialEvaluateCanExecute(object? parameter)
        {
            return _canExecute is null
                || _canExecute.Invoke(parameter).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        protected override async void OnExecute(object? parameter)
        {
            try
            {
                await _execute.Invoke(parameter);
            }
            catch (Exception exception) 
            {
                Logger.LogDebug(TraceLevel, "An exception was thrown and handled in an async void method.", exception.ToString());
            }
        }
    }
}
