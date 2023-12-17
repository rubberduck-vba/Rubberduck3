using Rubberduck.SettingsProvider.Model.Editor.Tools;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using Rubberduck.UI.Windows;
using System.Linq;
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

            if (!shell.ToolWindows.Any(e => e.Title == _vm.Title) || _view is null)
            {
                if (_view is null)
                {
                    _view = new TView { DataContext = _vm };
                    _vm.Content = _view; // <~ FIXME WTF

                    // TODO get from workspace if available, otherwise get from settings/defaults:
                    switch (_vm.DockingLocation)
                    {
                        case DockingLocation.DockLeft:
                            _shell.View.LeftPaneToolTabs.AddToSource(_view);
                            break;
                        case DockingLocation.DockRight:
                            _shell.View.RightPaneToolTabs.AddToSource(_view);
                            break;
                        case DockingLocation.DockBottom:
                            _shell.View.BottomPaneToolTabs.AddToSource(_view);
                            break;
                    }
                }

                shell.ToolWindows.Add(_vm);
            }

            switch (_vm.DockingLocation)
            {
                case DockingLocation.DockLeft:
                    _shell.View.LeftPaneExpander.IsExpanded = true;
                    break;
                case DockingLocation.DockRight:
                    _shell.View.RightPaneExpander.IsExpanded = true;
                    break;
                case DockingLocation.DockBottom:
                    _shell.View.BottomPaneExpander.IsExpanded = true;
                    break;
            }

            await Task.CompletedTask;
        }
    }
}
