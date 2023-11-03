using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Unmanaged.UIContext
{
    public interface IUiContextProvider
    {
        bool IsExecutingInUiContext();
        SynchronizationContext UiContext { get; }
        TaskScheduler UiTaskScheduler { get; }
    }
}
