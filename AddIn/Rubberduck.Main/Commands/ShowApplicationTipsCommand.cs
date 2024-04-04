using Rubberduck.UI.Command;
using Rubberduck.UI.Services;
using Rubberduck.Unmanaged.Abstract;
using Rubberduck.VBEditor.UI.OfficeMenus;
using System.Threading.Tasks;

namespace Rubberduck.Main.Commands.ApplicationTips
{
    class ShowApplicationTipsCommand : ComCommandBase, IShowApplicationTipsCommand
    {
        public ShowApplicationTipsCommand(UIServiceHelper service, IVbeEvents vbeEvents)
            : base(service, vbeEvents)
        {
        }

        protected override Task OnExecuteAsync(object? parameter)
        {
            throw new System.NotImplementedException();
        }
    }
}