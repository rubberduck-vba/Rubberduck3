using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using System;
using System.Threading.Tasks;

namespace Rubberduck.Editor.Commands
{
    public class ShutdownServerCommand : CommandBase
    {
        private readonly Func<ILanguageClient?> _provider;

        public ShutdownServerCommand(UIServiceHelper service, Func<ILanguageClient?> provider)
            : base(service)
        {
            _provider = provider;
            AddToCanExecuteEvaluation(param => _provider.Invoke() != null);
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            var client = _provider.Invoke() ?? throw new InvalidOperationException("LanguageClient instance was unexpectedly unset.");
            await client.Shutdown();
        }
    }
}
