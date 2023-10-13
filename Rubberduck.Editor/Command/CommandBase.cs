using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;

namespace Rubberduck.Editor.Command
{

    [ComVisible(false)]
    public abstract class CommandBase : ICommand
    {
        private static readonly List<MethodBase> ExceptionTargetSites = new();

        protected CommandBase(ILogger logger, ISettingsProvider<RubberduckSettings> settings)
        {
            Logger = logger;
            SettingsProvider = settings;

            CanExecuteCondition = parameter => true;
            OnExecuteCondition = parameter => true;

            ShortcutText = string.Empty;
        }

        protected ISettingsProvider<RubberduckSettings> SettingsProvider { get; }
        protected ILogger Logger { get; }
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
            var traceLevel = SettingsProvider.Settings.LanguageServerSettings.TraceLevel.ToTraceLevel();
            var allowExecute = false;

            if (TimedAction.TryRun(() =>
            {
                allowExecute = CanExecuteCondition(parameter);
            }, out var elapsed, out var exception))
            {
                Logger.LogPerformance(traceLevel, $"{GetType().Name}.CanExecute completed.", elapsed);
            }
            else if (exception is not null)
            {
                Logger.LogError(traceLevel, exception);
                if (exception.TargetSite != null && !ExceptionTargetSites.Contains(exception.TargetSite))
                {
                    ExceptionTargetSites.Add(exception.TargetSite);
                }
                allowExecute = false;
            }
            return allowExecute;
        }

        public void Execute(object? parameter)
        {
            var traceLevel = SettingsProvider.Settings.LanguageServerSettings.TraceLevel.ToTraceLevel();

            if (TimedAction.TryRun(async () =>
            {
                await OnExecuteAsync(parameter);
            }, out var elapsed, out var exception))
            {
                Logger.LogPerformance(traceLevel, $"{GetType().Name}.Execute completed.", elapsed);
            }
            else if (exception is not null)
            {
                Logger.LogError(traceLevel, exception);
                if (exception.TargetSite != null && !ExceptionTargetSites.Contains(exception.TargetSite))
                {
                    ExceptionTargetSites.Add(exception.TargetSite);
                }
            }
        }

        public string ShortcutText { get; set; }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
