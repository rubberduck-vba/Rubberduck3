namespace Rubberduck.Core.Editor.Commands
{
    /// <summary>
    /// A command that notifies the language server about a code edit.
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    public abstract class CodeCommand<TParam> : EditorShellCommand<TParam>
        where TParam : class
    {
        /* we need SDK to reference OmniSharp.LanguageClient directly...
         * ...but we don't want to reference OmniSharp libraries in Rubberduck.Core.
         * Instead we'll inject an interface that Rubberduck.Client will implement.
         */
        //protected CodeCommand(ILanguageClientService lsp) 
        //{

        //}

        protected sealed override void OnExecute(object parameter)
        {
            base.OnExecute(parameter);
        }
    }
}
