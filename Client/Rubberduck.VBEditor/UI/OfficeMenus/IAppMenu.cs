using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.VBEditor.UI.OfficeMenus
{
    public interface IAppMenu
    {
        void Localize();
        void Initialize();
        Task EvaluateCanExecuteAsync(object parameter, CancellationToken token);
    }
}
