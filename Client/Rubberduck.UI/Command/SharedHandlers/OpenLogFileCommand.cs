using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using System;
using System.Threading.Tasks;

namespace Rubberduck.UI.Command.SharedHandlers
{
    public class OpenLogFileCommand : CommandBase
    {
        public OpenLogFileCommand(UIServiceHelper service)
            : base(service)
        {

        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            throw new NotImplementedException();
        }
    }
}
