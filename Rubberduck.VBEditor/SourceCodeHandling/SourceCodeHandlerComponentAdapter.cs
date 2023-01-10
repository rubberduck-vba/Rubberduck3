using Rubberduck.VBEditor.ComManagement;

namespace Rubberduck.VBEditor.SourceCodeHandling
{
    public class SourceCodeHandlerComponentAdapter : ISourceCodeHandler<string>
    {
        private readonly IComponentSourceCodeHandler _componentSourceCodeHandler;
        private readonly IProjectsProvider _projectsProvider;

        public SourceCodeHandlerComponentAdapter(IComponentSourceCodeHandler componentSourceCodeHandler, IProjectsProvider projectsProvider)
        {
            _componentSourceCodeHandler = componentSourceCodeHandler;
            _projectsProvider = projectsProvider;
        }

        public int GetContentHash(QualifiedModuleName module)
        {
            var component = _projectsProvider.Component(module);
            if (component is null)
            {
                return 0;
            }

            return component.ContentHash();
        }

        public CodeString GetCurrentLogicalLine(QualifiedModuleName module)
        {
            throw new System.NotSupportedException();
        }

        public void SetSelection(QualifiedModuleName module, Selection selection)
        {
            throw new System.NotSupportedException();
        }

        public string SourceCode(QualifiedModuleName module)
        {
            var component = _projectsProvider.Component(module);
            if (component is null)
            {
                return string.Empty;
            }

            return _componentSourceCodeHandler.SourceCode(component);
        }

        public string StringSource(QualifiedModuleName module) => SourceCode(module);

        public void SubstituteCode(QualifiedModuleName module, string newCode)
        {
            var component = _projectsProvider.Component(module);
            if (component is null)
            {
                return;
            }

            using (_componentSourceCodeHandler.SubstituteCode(component, newCode)) 
            { /*We do nothing; we just need to guarantee that the returned RCW gets disposed.*/}
        }

        public void SubstituteCode(QualifiedModuleName module, CodeString newCode)
        {
            var component = _projectsProvider.Component(module);
            if (component is null)
            {
                return;
            }

            using (_componentSourceCodeHandler.SubstituteCode(component, newCode.Code)) 
            { /*We do nothing; we just need to guarantee that the returned RCW gets disposed.*/ }
        }
    }
}