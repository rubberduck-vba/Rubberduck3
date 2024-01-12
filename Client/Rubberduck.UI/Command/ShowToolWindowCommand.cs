using Rubberduck.SettingsProvider.Model.Editor.Tools;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using Rubberduck.UI.Windows;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Rubberduck.UI.Command
{
    public abstract class ShowToolWindowCommand<TView, TViewModel> : CommandBase
        where TView : UserControl, new()
        where TViewModel : class, IToolWindowViewModel
    {
        private readonly ShellProvider _shell;
        private readonly TViewModel _vm;

        public ShowToolWindowCommand(UIServiceHelper service, ShellProvider shell, TViewModel vm)
            : base(service)
        {
            _shell = shell;
            _vm = vm;
        }

        private TView? _view;
        protected async override Task OnExecuteAsync(object? parameter)
        {
            var shell = _shell.ViewModel;
            _vm.IsSelected = true;

            _vm.ContentControl = _view ??= new TView() { DataContext = _vm };

            switch (_vm.DockingLocation)
            {
                case DockingLocation.DockLeft:
                    shell.LeftPanelToolWindows.Add(_vm);
                    _shell.View.LeftPaneExpander.IsExpanded = true;
                    break;
                case DockingLocation.DockRight:
                    shell.RightPanelToolWindows.Add(_vm);
                    _shell.View.RightPaneExpander.IsExpanded = true;
                    break;
                case DockingLocation.DockBottom:
                    shell.BottomPanelToolWindows.Add(_vm);
                    _shell.View.BottomPaneExpander.IsExpanded = true;
                    break;
            }

            _shell.View.InvalidateVisual();
            await Task.CompletedTask;
        }
    }
}
