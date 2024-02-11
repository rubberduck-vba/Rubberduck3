using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Rubberduck.Unmanaged.NonDisposingDecorators;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers;
using Microsoft.Extensions.Logging;
using Rubberduck.Unmanaged.Model.Abstract;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB.Enums;

namespace Rubberduck.Unmanaged
{
    public class ProjectsRepository : IProjectsRepository
    {
        private IVBProjects _projectsCollection;
        private readonly Dictionary<Uri, IVBProject> _projects = new();
        private readonly Dictionary<Uri, IVBComponents> _componentsCollections = new();
        private readonly Dictionary<IQualifiedModuleName, IVBComponent> _components = new();
        private readonly Dictionary<Uri, IVBProject> _lockedProjects = new();

        private readonly ReaderWriterLockSlim _refreshProtectionLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        private readonly ILogger _logger;

        public ProjectsRepository(IVBE vbe, ILogger<ProjectsRepository> logger)
        {
            _projectsCollection = vbe.VBProjects;
            _logger = logger;
        }

        private void LoadCollections()
        {
            LoadProjects();
            LoadComponentsCollections();
            LoadComponents();
        }

        private void LoadProjects()
        {
            foreach (var project in _projectsCollection)
            {
                if (project.Protection == ProjectProtection.Locked)
                {
                    if (!TryStoreLockedProject(project))
                    {
                        project.Dispose();
                    }
                }
                else
                {
                    _projects.Add(project.Uri, project);
                }
            }
        }

        private bool TryStoreLockedProject(IVBProject project)
        {
            if (!project.TryGetFullPath(out var path))
            {
                _logger.LogWarning("Path of locked project could not be read.");
                return false;
            }

            _lockedProjects.Add(project.Uri, project);
            return true;
        }

        private void LoadComponentsCollections()
        {
            foreach (var kvp in _projects)
            {
                _componentsCollections.Add(kvp.Key, kvp.Value.VBComponents);
            }
        }

        private void LoadComponents()
        {
            foreach (var components in _componentsCollections.Values)
            {
                LoadComponents(components);
            }
        }

        private void LoadComponents(IVBComponents componentsCollection)
        {
            foreach (var component in componentsCollection)
            {
                var qmn = component.QualifiedModuleName;
                _components.Add(qmn, component);
            }
        }

        public void Refresh()
        {
            ExecuteWithinWriteLock(() => RefreshCollections());
        }

        private void ExecuteWithinWriteLock(Action action)
        {
            if (_disposed)
            {
                return; //The lock has already been disposed.
            }

            var writeLockTaken = false;
            try
            {
                _refreshProtectionLock.EnterWriteLock();
                writeLockTaken = true;
                action.Invoke();
            }
            finally
            {
                if (writeLockTaken)
                {
                    _refreshProtectionLock.ExitWriteLock();
                }
            }
        }

        private void RefreshCollections()
        {
            //We save a copy of the collections and only refresh after the collections have been loaded again
            //to avoid disconnecting any RCWs from the underlying COM object for objects that still exist.
            var projects = ClearComWrapperDictionary(_projects);
            var componentCollections = ClearComWrapperDictionary(_componentsCollections);
            var components = ClearComWrapperDictionary(_components);
            var lockedProjects = ClearComWrapperDictionary(_lockedProjects);

            try
            {
                LoadCollections();
            }
            finally
            {
                DisposeWrapperEnumerable(projects);
                DisposeWrapperEnumerable(componentCollections);
                DisposeWrapperEnumerable(components);
                DisposeWrapperEnumerable(lockedProjects);
            }
        }

        private IEnumerable<TWrapper> ClearComWrapperDictionary<TKey, TWrapper>(IDictionary<TKey, TWrapper> dictionary)
            where TWrapper : ISafeComWrapper
        {
            var copy = dictionary.Values.ToList();
            dictionary.Clear();
            return copy;
        }

        private void DisposeWrapperEnumerable<TWrapper>(IEnumerable<TWrapper> wrappers) where TWrapper : ISafeComWrapper
        {
            foreach (var wrapper in wrappers)
            {
                wrapper.Dispose();
            }
        }

        private void RefreshCollections(Uri uri)
        {
            if (!_projects.TryGetValue(uri, out var project))
            {
                return;
            }

            var componentsCollection = _componentsCollections[uri];
            var components = _components.Where(kvp => kvp.Key.WorkspaceUri.Equals(uri)).ToList();

            foreach (var qmn in components.Select(kvp => kvp.Key))
            {
                _components.Remove(qmn);
            }

            try
            {
                _componentsCollections[uri] = project.VBComponents;
                LoadComponents(_componentsCollections[uri]);
            }
            finally
            {
                componentsCollection.Dispose();
                DisposeWrapperEnumerable(components.Select(kvp => kvp.Value));
            } 
        }

        public void Refresh(Uri uri)
        {
            ExecuteWithinWriteLock(() => RefreshCollections(uri));
        }

        public IVBProjects ProjectsCollection()
        {
            return _projectsCollection != null 
                ? new VBProjectsNonDisposingDecorator<IVBProjects>(_projectsCollection) 
                : null;
        }

        public IEnumerable<(Uri Uri, IVBProject Project)> Projects()
        {
            return EvaluateWithinReadLock(() => _projects
                        .Select(kvp => (kvp.Key, new VBProjectNonDisposingDecorator<IVBProject>(kvp.Value) as IVBProject))
                        .ToList())
                ?? Enumerable.Empty<(Uri, IVBProject)>();
        }

        public IEnumerable<(Uri Uri, IVBProject Project)> LockedProjects()
        {
            return EvaluateWithinReadLock(() => _lockedProjects
                        .Select(kvp => (kvp.Key, new VBProjectNonDisposingDecorator<IVBProject>(kvp.Value) as IVBProject))
                        .ToList())
                ?? Enumerable.Empty<(Uri, IVBProject)>();
        }

        private T EvaluateWithinReadLock<T>(Func<T> function) where T: class
        {
            if (_disposed)
            {
                return default; //The lock has already been disposed.
            }

            var readLockTaken = false;
            try
            {
                _refreshProtectionLock.EnterReadLock();
                readLockTaken = true;
                return function.Invoke();
            }
            finally
            {
                if (readLockTaken)
                {
                    _refreshProtectionLock.ExitReadLock();
                }
            }
        }

        public IVBProject Project(Uri uri)
        {
            if (uri == null)
            {
                return null;
            }

            return EvaluateWithinReadLock(() => _projects.TryGetValue(uri, out var project) 
                ? new VBProjectNonDisposingDecorator<IVBProject>(project)
                : null);
        }

        public IVBComponents ComponentsCollection(Uri uri)
        {
            if (uri == null)
            {
                return null;
            }

            return EvaluateWithinReadLock(() => _componentsCollections.TryGetValue(uri, out var componentsCollection) 
                ? new VBComponentsNonDisposingDecorator<IVBComponents>(componentsCollection)
                : null);
        }

        public IEnumerable<(IQualifiedModuleName QualifiedModuleName, IVBComponent Component)> Components()
        {
            return EvaluateWithinReadLock(() => _components
                        .Select(kvp => (kvp.Key as IQualifiedModuleName, new VBComponentNonDisposingDecorator<IVBComponent>(kvp.Value) as IVBComponent))
                        .ToList()) 
                ?? new List<(IQualifiedModuleName, IVBComponent)>();
        }

        public IEnumerable<(IQualifiedModuleName QualifiedModuleName, IVBComponent Component)> Components(Uri uri)
        {
            return EvaluateWithinReadLock(() => _components.Where(kvp => kvp.Key.WorkspaceUri.Equals(uri))
                       .Select(kvp => (kvp.Key as IQualifiedModuleName, new VBComponentNonDisposingDecorator<IVBComponent>(kvp.Value) as IVBComponent))
                       .ToList())
                   ?? new List<(IQualifiedModuleName, IVBComponent)>();
        }

        public IVBComponent Component(IQualifiedModuleName qualifiedModuleName)
        {
            return EvaluateWithinReadLock(() => _components.TryGetValue(qualifiedModuleName, out var component) 
                ? new VBComponentNonDisposingDecorator<IVBComponent>(component) as IVBComponent
                : null);
        }

        public void RemoveComponent(IQualifiedModuleName qualifiedModuleName)
        {
            ExecuteWithinWriteLock(() =>
            {
                if (!_components.TryGetValue(qualifiedModuleName, out var component) ||
                    !_componentsCollections.TryGetValue(qualifiedModuleName.WorkspaceUri, out var componentsCollectionItem))
                {
                    return;
                }

                _components.Remove(qualifiedModuleName); // Remove our cached copy of the component
                componentsCollectionItem.Remove(component); // Remove the actual component from the project 
            });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed || !disposing)
            {
                return;
            }
            
            ExecuteWithinWriteLock(() => ClearCollections());

            _disposed = true;

            _projectsCollection.Dispose();
            _projectsCollection = null;

            _refreshProtectionLock.Dispose();
        }

        private void ClearCollections()
        {
            var projects = ClearComWrapperDictionary(_projects);
            var componentCollections = ClearComWrapperDictionary(_componentsCollections);
            var components = ClearComWrapperDictionary(_components);
            var lockedProjects = ClearComWrapperDictionary(_lockedProjects);

            DisposeWrapperEnumerable(projects);
            DisposeWrapperEnumerable(componentCollections);
            DisposeWrapperEnumerable(components);
            DisposeWrapperEnumerable(lockedProjects);
        }
    }
}
