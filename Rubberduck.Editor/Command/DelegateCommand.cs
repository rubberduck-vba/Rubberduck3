using Microsoft.Extensions.Logging;
using Rubberduck.SettingsProvider.Model;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Rubberduck.InternalApi.Settings;

namespace Rubberduck.Editor.Command
{
    [ComVisible(false)]
    public class DelegateCommand : CommandBase
    {
        private readonly Predicate<object?>? _canExecute;
        private readonly Action<object?> _execute;

        public DelegateCommand(ILogger logger, ISettingsProvider<RubberduckSettings> settings, Action<object?> execute, Predicate<object?>? canExecute = null)
            : base(logger, settings)
        {
            _canExecute = canExecute;
            _execute = execute;

            AddToCanExecuteEvaluation(SpecialEvaluateCanExecute);
        }

        private bool SpecialEvaluateCanExecute(object? parameter) => _canExecute is null || _canExecute.Invoke(parameter);

        protected override void OnExecute(object? parameter) => _execute.Invoke(parameter);
    }
}
