using Rubberduck.UI.Abstract;
using Rubberduck.UI.Command;
using System;

namespace Rubberduck.Core.Editor.Commands
{
    public abstract class EditorShellCommand<TParam> : CommandBase, IEditorShellCommand
        where TParam : class
    {
        public IEditorShellViewModel Shell { get; set; }

        protected override void OnExecute(object parameter)
        {
            var shell = Shell ?? throw new InvalidOperationException($"{nameof(Shell)} is not initialized.");
            var param = parameter as TParam ?? throw new ArgumentException($"Unexpected parameter type. Was {parameter.GetType().Name}, expected {typeof(TParam).Name}");

            ExecuteInternal(shell, param);
        }

        protected abstract void ExecuteInternal(IEditorShellViewModel shell, TParam param);
    }
}
