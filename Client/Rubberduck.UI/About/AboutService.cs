namespace Rubberduck.UI.About
{
    public class AboutService : WindowService<AboutWindow, IAboutWindowViewModel>
    {
        public AboutService(IAboutWindowViewModel viewModel)
            : base(viewModel)
        {
        }

        protected override AboutWindow CreateWindow(IAboutWindowViewModel model) => new(model);
    }
}
