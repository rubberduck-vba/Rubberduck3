namespace Rubberduck.UI.NewProject
{
    public class NewProjectWindowFactory : IWindowFactory<NewProjectWindow, NewProjectWindowViewModel>
    {
        public NewProjectWindow Create(NewProjectWindowViewModel model) => new(model);
    }
}
