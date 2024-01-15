using Microsoft.Extensions.Logging;
using Rubberduck.SettingsProvider;
using Rubberduck.Unmanaged.UIContext;
using System;
using System.Threading;
using System.Windows.Threading;

namespace Rubberduck.UI.Services
{
    public class UserFacingExceptionEventArgs : EventArgs
    {
        public UserFacingExceptionEventArgs(Exception exception, string? message)
        {
            Exception = exception;
            Message = message;
        }

        public Exception Exception { get; init; }
        public string? Message { get; init; }
    }

    public class UIServiceHelper : ServiceBase
    {
        private readonly SynchronizationContext _uiContext;

        public event EventHandler<UserFacingExceptionEventArgs> UserFacingException = delegate { };

        public UIServiceHelper(
            IUiContextProvider uiDispatcher,
            ILogger<UIServiceHelper> logger, 
            RubberduckSettingsProvider settingsProvider, 
            PerformanceRecordAggregator performance) 
            : base(logger, settingsProvider, performance)
        {
            SettingsProvider = settingsProvider;
            _uiContext = uiDispatcher.UiContext;
        }

        public void RunOnMainThread(Action action)
        {
            _uiContext.Post(state => action(), null);
        }

        public new RubberduckSettingsProvider SettingsProvider { get; }

        protected virtual void OnUserFacingException(Exception exception, string? message)
        {
            UserFacingException?.Invoke(this, new UserFacingExceptionEventArgs(exception, message));
        }

        protected override void OnError(Exception exception, string? message)
        {
            base.OnError(exception, message);
            OnUserFacingException(exception, message);
        }
    }
}