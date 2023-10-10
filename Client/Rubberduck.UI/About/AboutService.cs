using Rubberduck.InternalApi.Model.Abstract;
using Rubberduck.UI.Abstract;
using Rubberduck.UI.Xaml.About;

namespace Rubberduck.UI.About
{
    public class AboutService : WindowService<AboutWindow, IAboutWindowViewModel>
    {
        public AboutService(IAboutWindowViewModel viewModel) 
            : base(viewModel)
        {
        }

        protected override AboutWindow? CreateWindow(IAboutWindowViewModel model) => new AboutWindow(model);
    }
}
