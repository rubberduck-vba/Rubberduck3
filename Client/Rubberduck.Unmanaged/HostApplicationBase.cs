using Rubberduck.Unmanaged.Abstract.SafeComWrappers;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB.Enums;
using Rubberduck.Unmanaged.Model;
using Rubberduck.Unmanaged.Model.Abstract;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Rubberduck.Unmanaged
{
    [ComVisible(false)]
    public abstract class HostApplicationBase<TApplication> : SafeComWrapper<TApplication>, IHostApplication
        where TApplication : class
    {
        protected readonly IVBE Vbe;

        protected HostApplicationBase(IVBE vbe, string applicationName, bool useComReflection = false)
            : base(useComReflection
                ? ApplicationFromComReflection(applicationName)
                : ApplicationFromVbe(vbe, applicationName))
        {
            Vbe = vbe;
            ApplicationName = applicationName;
        }

        private static TApplication ApplicationFromComReflection(string applicationName)
        {
            TApplication application;
            try
            {
                application = (TApplication)Marshal.GetActiveObject($"{applicationName}.Application");
            }
            catch (COMException)
            {
                //logger.LogError(exception, "Unexpected COM exception while acquiring the host application object for application {applicationName} via COM reflection.", applicationName);
                application = null;
            }
            catch (InvalidCastException)
            {
                //TODO: Find out why this ever happens.
                //logger.LogError(exception, "Unable to cast the host application object for application {applicationName} acquired via COM reflection to its PIA type.", applicationName);
                application = null;
            }
            catch (Exception)
            {
                //note: We catch all exceptions because we currently really do not need application object and there can be exceptions for unexpected system setups.
                //logger.LogError(exception, "Unexpected exception while acquiring the host application object for application {applicationName} from a document module.", applicationName);
                application = null;
            }

            return application;
        }

        private static TApplication ApplicationFromVbe(IVBE vbe, string applicationName)
        {
            TApplication application;
            try
            {
                using (var appProperty = ApplicationPropertyFromDocumentModule(vbe))
                {
                    if (appProperty != null)
                    {
                        application = (TApplication)appProperty.Object;
                    }
                    else
                    {
                        application = ApplicationFromComReflection(applicationName);
                    }
                }

            }
            catch (COMException)
            {
                //logger.LogError(exception, "Unexpected COM exception while acquiring the host application object for application {applicationName} from a document module.", applicationName);
                application = null;
            }
            catch (InvalidCastException)
            {
                //logger.LogError(exception, "Unable to cast the host application object for application {applicationName} acquiered from a document module to its PIA type.", applicationName);
                application = null;
            }
            catch (Exception)
            {
                //note: We catch all exceptions because we currently really do not need application object and there can be exceptions for unexpected system setups.
                //logger.LogError(exception, "Unexpected exception while acquiring the host application object for application {applicationName} from a document module.", applicationName);
                application = null;
            }
            return application;
        }

        private static IProperty ApplicationPropertyFromDocumentModule(IVBE vbe)
        {
            using var projects = vbe.VBProjects;
            foreach (var project in projects)
            {
                using (project)
                {
                    if (project.Protection == ProjectProtection.Locked) continue;

                    using var components = project.VBComponents;
                    foreach (var component in components)
                    {
                        using (component)
                        {
                            if (component.Type != ComponentType.Document) continue;

                            using var properties = component.Properties;
                            if (properties.Count <= 1) continue;

                            foreach (var property in properties)
                                using (property)
                                {
                                    if (property.Name == "Application") return property;
                                }
                        }
                    }
                }
            }

            return null;
        }

        protected TApplication Application => Target;

        public string ApplicationName { get; }

        private const string ComponentName = "VBIDE.VBComponent";

        public virtual IEnumerable<HostDocument> GetDocuments()
        {
            var result = new List<HostDocument>();

            foreach (var document in DocumentComponents())
            {
                var moduleName = new QualifiedModuleName(document);
                var name = GetName(document);

                result.Add(new HostDocument(moduleName, name, ComponentName, VBDocumentState.DesignView, null));
            }
            return result;
        }

        public virtual HostDocument GetDocument(IQualifiedModuleName moduleName)
        {
            using (var projects = Vbe.VBProjects)
            {
                foreach (var project in projects)
                    using (project)
                    {
                        if (moduleName.ProjectName != project.Name || moduleName.WorkspaceUri != project.Uri)
                        {
                            continue;
                        }

                        using var components = project.VBComponents;
                        using var component = components[moduleName.ComponentName];
                        var name = GetName(component);
                        return new HostDocument(moduleName, name, ComponentName, VBDocumentState.DesignView, null);
                    }
            }

            return null;
        }

        public virtual bool CanOpenDocumentDesigner(IQualifiedModuleName moduleName)
        {
            return false;
        }

        public virtual bool TryOpenDocumentDesigner(IQualifiedModuleName moduleName)
        {
            return false;
        }

        public virtual IEnumerable<HostAutoMacro> AutoMacroIdentifiers => new HostAutoMacro[] { };

        private static string GetName(IVBComponent component)
        {
            var name = string.Empty;
            try
            {
                using var properties = component.Properties;
                using var nameProperty = properties["Name"];
                name = nameProperty?.Value.ToString() ?? string.Empty;
            }
            catch (Exception)
            {
                //logger.LogTrace(exception, "An exception attempting to access VBComponent.Properties.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                name = component.Name;
            }

            return name;
        }

        protected IEnumerable<IVBComponent> DocumentComponents()
        {
            using var projects = Vbe.VBProjects;
            foreach (var project in projects)
            {
                using (project)
                using (var components = project.VBComponents)
                {
                    foreach (var component in components)
                        using (component)
                        {
                            if (component.Type == ComponentType.Document)
                            {
                                yield return component;
                            }
                        }
                }
            }
        }

        public override bool Equals(ISafeComWrapper<TApplication> other)
        {
            return IsEqualIfNull(other) || other != null && ReferenceEquals(other.Target, Target);
        }

        public override int GetHashCode()
        {
            return IsWrappingNullReference ? 0 : HashCode.Combine(Target);
        }

        protected override void Dispose(bool disposing) => base.Dispose(disposing);

        ~HostApplicationBase()
        {
            Dispose(false);
        }
    }
}
