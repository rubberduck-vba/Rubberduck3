using Rubberduck.UI.NewProject;
using Rubberduck.UI.Windows;

namespace Rubberduck.Editor.DialogServices.NewProject
{
    public class NewProjectWindowFactory : IWindowFactory<NewProjectWindow, NewProjectWindowViewModel>
    {
        public NewProjectWindow Create(NewProjectWindowViewModel model) => new(model);
    }
}
