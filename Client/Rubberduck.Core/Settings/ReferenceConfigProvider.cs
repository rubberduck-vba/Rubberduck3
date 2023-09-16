using System;
using System.IO.Abstractions;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rubberduck.Resources.Registration;
using Rubberduck.SettingsProvider;
using Rubberduck.UI;
using Rubberduck.VBEditor;
using Rubberduck.VBEditor.Events;
using Rubberduck.VBEditor.SafeComWrappers;

namespace Rubberduck.Settings
{
    public class ReferenceConfigProvider : ConfigurationServiceBase<ReferenceSettings>, IDisposable
    {
        private readonly string _hostApplication;
        
        private readonly IEnvironmentProvider _environment;
        private readonly IVbeEvents _events;
        private readonly IFileSystem _filesystem;
        private bool _listening;

        public ReferenceConfigProvider(
            IAsyncPersistenceService<ReferenceSettings> persister, 
            IEnvironmentProvider environment, 
            IVbeEvents events,
            IFileSystem filesystem)
            : base(persister, null /*new DefaultSettings<ReferenceSettings, Properties.Settings>()*/)
        {
            _environment = environment;
            _events = events;
            _filesystem = filesystem;
            _hostApplication = _filesystem.Path.GetFileName(Application.ExecutablePath).ToUpperInvariant();

            // FIXME move IO out of ctor

            //var settings = ReadAsync();
            //_listening = settings.AddToRecentOnReferenceEvents;
            //if (_listening && _events != null)
            //{
            //    _events.ProjectReferenceAdded += HandleReferenceAdded;
            //}
        }

        public async override Task<ReferenceSettings> ReadDefaultsAsync()
        {
            var defaults = ReferenceSettings.Default;

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            defaults.PinReference(new ReferenceInfo(new Guid(RubberduckGuid.RubberduckTypeLibGuid), string.Empty, string.Empty, version.Major, version.Minor));
            defaults.PinReference(new ReferenceInfo(new Guid(RubberduckGuid.RubberduckApiTypeLibGuid), string.Empty, string.Empty, version.Major, version.Minor));

            var documents = _environment?.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!string.IsNullOrEmpty(documents))
            {
                defaults.ProjectPaths.Add(documents);
            }

            var appdata = _environment?.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (!string.IsNullOrEmpty(documents))
            {
                var addins = _filesystem.Path.Combine(appdata, "Microsoft", "AddIns");
                if (_filesystem.Directory.Exists(addins))
                {
                    defaults.ProjectPaths.Add(addins);
                }

                var templates = _filesystem.Path.Combine(appdata, "Microsoft", "Templates");
                if (_filesystem.Directory.Exists(templates))
                {
                    defaults.ProjectPaths.Add(templates);
                }
            }

            return await Task.FromResult(defaults);
        }

        public async override Task SaveAsync(ReferenceSettings settings)
        {
            if (_listening && _events != null && !settings.AddToRecentOnReferenceEvents)
            {
                _events.ProjectReferenceAdded -= HandleReferenceAdded;
                _listening = false;
            }

            if (_listening && _events != null && !settings.AddToRecentOnReferenceEvents)
            {
                _events.ProjectReferenceAdded += HandleReferenceAdded;
                _listening = true;
            }
            OnSettingsChanged();
            await PersistValueAsync(settings);
        }

        private async void HandleReferenceAdded(object sender, ReferenceEventArgs e)
        {
            if (e is null || e.Reference.Equals(ReferenceInfo.Empty))
            {
                return;
            }

            var settings = await ReadAsync();
            settings.TrackUsage(e.Reference, e.Type == ReferenceKind.Project ? _hostApplication : null);
            await SaveAsync(settings);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing && _listening)
            {
                _events.ProjectReferenceAdded -= HandleReferenceAdded;
            }

            _disposed = true;
        }
    }
}
