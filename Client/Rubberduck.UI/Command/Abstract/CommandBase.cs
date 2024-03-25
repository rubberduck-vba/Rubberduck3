using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Rubberduck.UI.Services;

namespace Rubberduck.UI.Command.Abstract
{

    [ComVisible(false)]
    public abstract class CommandBase : ICommand
    {
        private readonly UIServiceHelper _service;

        protected CommandBase(UIServiceHelper service)
        {
            _service = service;

            CanExecuteCondition = parameter => true;
            OnExecuteCondition = parameter => true;

            ShortcutText = string.Empty;
            Name = GetType().Name;
        }

        public string Name { get; init; }

        protected UIServiceHelper Service => _service;
        protected abstract Task OnExecuteAsync(object? parameter);

        protected Func<object?, bool> CanExecuteCondition { get; private set; }
        protected Func<object?, bool> OnExecuteCondition { get; private set; }

        protected void AddToCanExecuteEvaluation(Func<object?, bool> furtherCanExecuteEvaluation, bool requireReevaluationAlso = false)
        {
            if (furtherCanExecuteEvaluation == null)
            {
                return;
            }

            var currentCanExecuteCondition = CanExecuteCondition;
            CanExecuteCondition = (parameter) =>
                currentCanExecuteCondition(parameter) && furtherCanExecuteEvaluation(parameter);

            if (requireReevaluationAlso)
            {
                AddToOnExecuteEvaluation(furtherCanExecuteEvaluation);
            }
        }

        protected void AddToOnExecuteEvaluation(Func<object?, bool> furtherCanExecuteEvaluation)
        {
            if (furtherCanExecuteEvaluation == null)
            {
                return;
            }

            var currentOnExecute = OnExecuteCondition;
            OnExecuteCondition = (parameter) =>
                currentOnExecute(parameter) && furtherCanExecuteEvaluation(parameter);
        }

        public bool CanExecute(object? parameter)
        {
            var allowExecute = false;

            _service.TryRunAction(() =>
            {
                allowExecute = CanExecuteCondition(parameter);
            }, $"{Name}.CanExecute");

            return allowExecute;
        }

        public void Execute(object? parameter)
        {
            _service.LogDebug($"Executing command: {Name}");
            _service.TryRunAction(() =>
            {
                _ = OnExecuteAsync(parameter);
            }, $"{Name}.Execute");
        }

        public string ShortcutText { get; set; }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public CanExecuteRoutedEventHandler CanExecuteRouted() =>
            (sender, e) => e.CanExecute = CanExecute(e.Parameter);

        public ExecutedRoutedEventHandler ExecutedRouted() =>
            (sender, e) => Execute(e.Parameter);
    }
}
