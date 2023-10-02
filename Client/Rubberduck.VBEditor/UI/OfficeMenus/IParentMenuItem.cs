using Rubberduck.Unmanaged.Abstract.SafeComWrappers.Office;
using System.Threading.Tasks;
using System.Threading;

namespace Rubberduck.VBEditor.UI.OfficeMenus
{
    public interface IParentMenuItem : IMenuItem
    {
        ICommandBarControls Parent { get; set; }
        ICommandBarPopup Item { get; }
        void RemoveMenu();
        int? BeforeIndex { get; set; }
        void AddChildItem(IMenuItem item);

        void Localize();
        void Initialize();
        Task EvaluateCanExecuteAsync(object parameter, CancellationToken token);
    }
}
