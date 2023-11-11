using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace Rubberduck.UI.Services
{
    public enum WindowSize
    {
        SmallWide,
        SmallTall,
        MediumDialog,
        LargeDialog,
    }

    public static class WindowSizeMap
    {
        public static IDictionary<WindowSize, Size> SizeMap = new Dictionary<WindowSize, Size>
        {
            [WindowSize.SmallWide] = new Size(600, 380),
            [WindowSize.SmallTall] = new Size(480, 600),
            [WindowSize.MediumDialog] = new Size(800, 600),
            [WindowSize.LargeDialog] = new Size(1280, 768),
        };
    }

    public abstract class WindowService<TView, TViewModel> : ServiceBase, IPresenter
        where TView : Window
        where TViewModel : class, INotifyPropertyChanged
    {
        private TView? _view;

        protected WindowService(ILogger<WindowService<TView, TViewModel>> logger, RubberduckSettingsProvider settings, TViewModel viewModel)
            : base(logger, settings)
        {
            Model = viewModel;
        }

        public TViewModel Model { get; }
        protected abstract TView CreateWindow(TViewModel model);

        public void Close() => _view?.Close();

        public void Hide() => _view?.Hide();

        protected virtual bool PreconditionCheck() => true;

        public void Show(WindowSize windowSize = WindowSize.MediumDialog)
        {
            TryRunAction(() =>
            {
                if (PreconditionCheck())
                {
                    _view ??= CreateWindow(Model);
                    var size = WindowSizeMap.SizeMap[windowSize];
                    _view.Height = size.Height;
                    _view.Width = size.Width;
                    _view.Show();
                }
                else
                {
                    LogDebug("Precondition check returned false; window will not be displayed.");
                }
            });
        }
    }
}
