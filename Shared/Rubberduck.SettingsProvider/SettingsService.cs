using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Resources;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider.Model.LanguageClient;
using Rubberduck.SettingsProvider.Model.LanguageServer;
using System;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rubberduck.SettingsProvider
{
    /// <summary>
    /// Abstracts file I/O operations for a provided <c>TSettings</c> type.
    /// </summary>
    /// <typeparam name="TSettings"></typeparam>
    public interface ISettingsService<TSettings> : ISettingsProvider<TSettings>
    {
        /// <summary>
        /// Reads and deserializes settings from disk into a <c>TSettings</c> value.
        /// </summary>
        /// <returns></returns>
        TSettings Read();

        /// <summary>
        /// Serializes the provided <c>TSettings</c> value to disk.
        /// </summary>
        void Write(TSettings settings);
    }

    public class SettingsService<TSettings> : ISettingsService<TSettings>
        where TSettings : NameValueSetting, new()
    {
        private static readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;
        private readonly TSettings _default;
        private readonly string _path;

        private readonly IServiceProvider _services;
        private GeneralSettingsGroup GeneralSettings => _services.GetRequiredService<ISettingsProvider<GeneralSettingsGroup>>().Settings;
        private LanguageClientSettingsGroup LanguageClientSettings => _services.GetRequiredService<ISettingsProvider<LanguageClientSettingsGroup>>().Settings;
        private LanguageServerSettingsGroup LanguageServerSettings => _services.GetRequiredService<ISettingsProvider<LanguageServerSettingsGroup>>().Settings;
        private UpdateServerSettingsGroup UpdateServerSettings => _services.GetRequiredService<ISettingsProvider<UpdateServerSettingsGroup>>().Settings;
        private TelemetryServerSettingsGroup TelemetryServerSettings => _services.GetRequiredService<ISettingsProvider<TelemetryServerSettingsGroup>>().Settings;

        private TSettings _cached;

        public SettingsService(ILogger<SettingsService<TSettings>> logger,
            IServiceProvider serviceProvider,
            IFileSystem fileSystem,
            IDefaultSettingsProvider<TSettings> defaultSettings)
        {
            _services = serviceProvider;
            _logger = logger;
            _fileSystem = fileSystem;

            var root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _path = _fileSystem.Path.Combine(root, "Rubberduck", $"{typeof(TSettings).Name}.json");

            _default = defaultSettings.Default;
            _cached = _default;
        }

        public event EventHandler<SettingsChangedEventArgs<TSettings>>? SettingsChanged;

        private void OnSettingsChanged(TSettings oldValue) => SettingsChanged?.Invoke(this, new SettingsChangedEventArgs<TSettings>(oldValue, Settings));

        public void ClearCache()
        {
            _cached = _default;
            Read();
        }

        public TSettings Settings => _cached;

        public TraceLevel TraceLevel => GeneralSettings.TraceLevel.ToTraceLevel();

        private bool TrySetValue(TSettings value)
        {
            var traceLevel = TraceLevel;
            var didChange = false;

            var oldValue = _cached;
            if (!value.Equals(oldValue))
            {
                _cached = value;
                didChange = true;
                _logger.LogInformation(traceLevel, "Settings have changed.");
                OnSettingsChanged(oldValue);
            }

            return didChange;
        }

        public TSettings Read()
        {
            var traceLevel = TraceLevel;
            _logger.LogTrace(traceLevel, "Reading settings from file...", _path);

            try
            {
                var path = _path;
                var root = ApplicationConstants.RUBBERDUCK_FOLDER_PATH;

                if (!_fileSystem.Directory.Exists(root))
                {
                    _fileSystem.Directory.CreateDirectory(root);
                    _logger.LogInformation(traceLevel, "Root application folder was created.", root);
                }

                if (_fileSystem.File.Exists(path))
                {
                    if (TimedAction.TryRun(async () =>
                    {
                        await Task.Run(() =>
                        {
                            var content = _fileSystem.File.ReadAllText(path);
                            _logger.LogTrace(traceLevel, "File content successfully read from file.", $"File: '{path}'");

                            if (!string.IsNullOrWhiteSpace(content))
                            {
                                var newValue = JsonSerializer.Deserialize<TSettings>(content, _options) ?? new();
                                _logger.LogTrace(traceLevel, "File content successfully deserialized.");
                                TrySetValue(newValue);
                            }
                            else
                            {
                                _logger.LogWarning(traceLevel, "File was found, but no content was loaded.");
                            }
                        });
                    }, out var elapsed, out var exception))
                    {
                        _logger.LogPerformance(traceLevel, "Deserialized settings from file.", elapsed);
                    }
                    else if (exception != default)
                    {
                        _logger.LogError(traceLevel, exception);
                    }
                }
                else
                {
                    _logger.LogWarning(traceLevel, "Settings file does not exist and will be created from defaults.", $"Path: '{path}'");
                    Write(_default);
                }
            }
            catch (Exception exception)
            {
                _logger.LogWarning(traceLevel, "Error reading from settings file. Cached settings remain in effect.", $"Path: '{_path}'");
                _logger.LogError(traceLevel, exception);
                throw;
            }

            return Settings;
        }

        public void Write(TSettings settings)
        {
            var traceLevel = TraceLevel;
            _logger.LogTrace(traceLevel, "Writing to settings file...", _path);

            var path = _path;
            var fileSystem = _fileSystem;

            var success = TimedAction.TryRun(async () =>
            {
                var content = JsonSerializer.Serialize(settings, _options);
                await Task.Run(() => fileSystem.File.WriteAllText(path, content));

            }, out var elapsed, out var exception);
            
            if (success)
            {
                _logger.LogPerformance(traceLevel, $"Serializing and writing content to file '{_path}' completed.", elapsed);
            }
            else if (exception != default)
            {
                _logger.LogError(traceLevel, exception);
            }
        }
    }
}