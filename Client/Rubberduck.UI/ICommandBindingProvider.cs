using System.Collections.Generic;
using System.Windows.Input;

namespace Rubberduck.UI
{
    public interface ICommandBindingProvider
    {
        IEnumerable<CommandBinding> CommandBindings { get; }
    }
}
