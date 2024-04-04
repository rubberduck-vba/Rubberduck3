using System;
using System.Runtime.InteropServices;

//using NLog;
using Rubberduck.Unmanaged;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB;
using Rubberduck.Unmanaged.Abstract.SourceCodeProvider;
using Rubberduck.Unmanaged.Model;

namespace Rubberduck.VBEditor.Utility
{
    public class AddComponentService : IAddComponentService
    {
        private readonly IProjectsProvider _projectsProvider;
        private readonly IComponentSourceCodeHandler _codePaneSourceCodeHandler;
        private readonly IComponentSourceCodeHandler _attributeSourceCodeHandler;

        //private static ILogger _logger = LogManager.GetCurrentClassLogger();

        public AddComponentService(
            IProjectsProvider projectsProvider,
            IComponentSourceCodeHandler codePaneComponentSourceCodeProvider,
            IComponentSourceCodeHandler attributesComponentSourceCodeProvider)
        {
            _projectsProvider = projectsProvider;
            _codePaneSourceCodeHandler = codePaneComponentSourceCodeProvider;
            _attributeSourceCodeHandler = attributesComponentSourceCodeProvider;
        }

        public void AddComponent(Uri workspaceUri, ComponentType componentType, string? code = null, string? additionalPrefixInModule = null, string? componentName = null)
        {
            AddComponent(_codePaneSourceCodeHandler, workspaceUri, componentType, code, additionalPrefixInModule, componentName);
        }

        public void AddComponentWithAttributes(Uri workspaceUri, ComponentType componentType, string code, string? prefixInModule = null, string? componentName = null)
        {
            AddComponent(_attributeSourceCodeHandler, workspaceUri, componentType, code, prefixInModule, componentName);
        }

        public void AddComponent(IComponentSourceCodeHandler sourceCodeHandler, Uri workspaceUri, ComponentType componentType, string? code = null, string? prefixInModule = null, string? componentName = null)
        {
            using var newComponent = CreateComponent(workspaceUri, componentType);
            if (newComponent is null)
            {
                return;
            }

            if (code != null)
            {
                using var loadedComponent = sourceCodeHandler.SubstituteCode(newComponent, code);
                AddPrefix(loadedComponent, prefixInModule);
                RenameComponent(loadedComponent, componentName);
                ShowComponent(loadedComponent);
            }
            else
            {
                AddPrefix(newComponent, prefixInModule);
                RenameComponent(newComponent, componentName);
                ShowComponent(newComponent);
            }
        }

        private static void RenameComponent(IVBComponent newComponent, string? componentName)
        {
            if (componentName is null)
            {
                return;
            }

            try
            {
                newComponent.Name = componentName;
            }
            catch (COMException)
            {
                //_logger.Debug(ex, $"Unable to rename component to {componentName}.");
            }
        }

        private static void ShowComponent(IVBComponent component)
        {
            if (component is null)
            {
                return;
            }

            using var codeModule = component.CodeModule;
            if (codeModule is null)
            {
                return;
            }

            using var codePane = codeModule.CodePane;
            codePane.Show();
        }

        private static void AddPrefix(IVBComponent module, string? prefix)
        {
            if (prefix is null || module is null)
            {
                return;
            }

            using var codeModule = module.CodeModule;
            codeModule.InsertLines(1, prefix);
        }

        private IVBComponent CreateComponent(Uri workspaceUri, ComponentType componentType)
        {
            var componentsCollection = _projectsProvider.ComponentsCollection(workspaceUri);
            if (componentsCollection is null)
            {
                return null!;
            }

            return componentsCollection.Add(componentType);
        }
    }
}