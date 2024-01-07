using Rubberduck.UI.NewProject;
using Rubberduck.UI.Windows;

namespace Rubberduck.UI.Services.NewProject
{
    public class NewProjectWindowFactory : IWindowFactory<NewProjectWindow, NewProjectWindowViewModel>
    {
        public NewProjectWindow Create(NewProjectWindowViewModel model) => new(model);
    }
}
