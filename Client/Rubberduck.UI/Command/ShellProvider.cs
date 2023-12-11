using Microsoft.Extensions.DependencyInjection;
using Rubberduck.UI.Shell;
using System;

namespace Rubberduck.UI.Command
{
    public class ShellProvider
    {
        private readonly IServiceProvider _provider;
        private ShellWindow? _shell;

        public ShellProvider(IServiceProvider provider) 
        {
            _provider = provider;
        }

        public IShellWindowViewModel ViewModel => _provider.GetRequiredService<IShellWindowViewModel>();
        public ShellWindow View => _shell ??= _provider.GetRequiredService<Func<ShellWindow>>().Invoke();
    }
}
