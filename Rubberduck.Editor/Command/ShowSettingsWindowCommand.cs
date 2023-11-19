using Rubberduck.UI.Command;
using Rubberduck.UI.Services;
using System;
using System.Threading.Tasks;

namespace Rubberduck.Editor.Command
{
    public class ShowSettingsWindowCommand : CommandBase
    {
        public ShowSettingsWindowCommand(UIServiceHelper service) 
            : base(service)
        {
        }

        protected override Task OnExecuteAsync(object? parameter)
        {
            throw new NotImplementedException();
        }
    }
}
