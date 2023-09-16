using Rubberduck.UI.Abstract;
using Rubberduck.UI.Command;
using System;
using System.Threading.Tasks;

namespace Rubberduck.Core.Editor.Commands
{
    public abstract class EditorShellCommand<TParam> : CommandBase, IEditorShellCommand
        where TParam : class
    {
        protected async override Task OnExecuteAsync(object parameter)
        {
            var shell = EditorShellContext.Current.Shell ?? throw new InvalidOperationException($"Editor shell is not initialized.");
            var param = parameter as TParam ?? throw new ArgumentException($"Unexpected parameter type. Was {parameter.GetType().Name}, expected {typeof(TParam).Name}");

            await Task.Run(() => ExecuteInternal(shell, param));
        }

        protected abstract void ExecuteInternal(IEditorShellViewModel shell, TParam param);
    }
}
