using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.Resources;
using Rubberduck.SettingsProvider.Model;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rubberduck.SettingsProvider
{
    public interface ISettingsService<TSettings> : ISettingsProvider<TSettings>
        where TSettings : struct
    {
        event EventHandler<SettingsChangedEventArgs<TSettings>> SettingsChanged;
        (Guid Token, TSettings Settings) ReadFromFile();
        void WriteToFile();
    }

    public class SettingsService<TSettings> : ISettingsService<TSettings>
        where TSettings : struct
    {
        private static readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;
        private readonly TSettings _default;
        private readonly string _path;

        private readonly ConcurrentDictionary<Guid, TSettings> _valueCache = new();
        private readonly ConcurrentStack<Guid> _tokens = new();

        private readonly IServiceProvider _services;
        LanguageServerSettings ServerSettings => _services.GetRequiredService<ISettingsProvider<LanguageServerSettings>>().Value.Settings;

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

            DefaultToken = Guid.NewGuid();
            CacheDefault();
        }

        public event EventHandler<SettingsChangedEventArgs<TSettings>>? SettingsChanged;

        public Guid DefaultToken { get; }

        private void OnSettingsChanged(TSettings oldValue, TSettings newValue, Guid token)
        {
            var args = new SettingsChangedEventArgs<TSettings>(oldValue, newValue, token);
            SettingsChanged?.Invoke(this, args);
        }

        public void ClearCache()
        {
            _tokens.Clear();
            _valueCache.Clear();
            CacheDefault();
            ReadFromFile();
        }

        private void CacheDefault()
        {
            _tokens.Push(DefaultToken);
            _valueCache[DefaultToken] = _default;
        }

        public (Guid Token, TSettings Settings) Value
        {
            get
            {
                var token = CurrentToken;
                var value = _valueCache[token];
                return (token, value);
            }
        }

        public Guid CurrentToken
        {
            get
            {
                if (_tokens.TryPeek(out var token))
                {
                    return token;
                }
                throw new InvalidOperationException("No token is set.");
            }
        }

        public bool TryGetValue(Guid token, out TSettings value) => _valueCache.TryGetValue(token, out value);

        public bool TrySetValue(TSettings value, Guid token)
        {
            var traceLevel = ServerSettings.TraceLevel.ToTraceLevel();

            var didChange = false;
            var oldToken = CurrentToken;

            if (_valueCache.TryGetValue(oldToken, out var oldValue))
            {
                if (Guid.Equals(token, oldToken))
                {
                    // if tokens match, caller was working off the same "current" we're looking at.

                    if (!_valueCache[token].Equals(value))
                    {
                        // different values at the specified token means settings did change and we're getting a new token.
                        var newToken = Guid.NewGuid();

                        _valueCache[newToken] = value;
                        _tokens.Push(newToken);

                        _logger.LogTrace($"Cached new {typeof(TSettings).Name} value.", $"new token: {newToken}.", traceLevel);
                        didChange = true;
                    }
                }
                else
                {
                    _logger.LogDebug("A stale token was provided; value will not be set.");
                    didChange = false;
                }

                if (didChange)
                {
                    Debug.Assert(!oldValue.Equals(value), "BUG: 'didChange' flag is set, but 'oldValue.Equals(value)' returned 'true'.");
                    var newToken = CurrentToken;

                    Debug.Assert(oldToken.Equals(newToken), "BUG: 'didChange' flag is set, but 'token.Equals(newToken)' returned 'false'.");
                    OnSettingsChanged(oldValue, value, newToken);
                }
            }
            else
            {
                // the "old" token should be mapped to an existing current / "old" value.
                // we should consider the token as stale rather than invalid in this case
                // because an unmapped token could be because cache was cleared since token retrieval.
                _logger.LogDebug("A stale or otherwise invalid token was provided; value will not be set.");
            }
            return didChange;
        }

        private string RootPath => ApplicationConstants.RUBBERDUCK_FOLDER_PATH;

        public (Guid Token, TSettings Settings) ReadFromFile()
        {
            var traceLevel = ServerSettings.TraceLevel.ToTraceLevel();
            _logger.LogTrace("Reading settings from file...", _path, traceLevel);

            try
            {
                var path = _path;
                var root = RootPath;
                if (!_fileSystem.Directory.Exists(root))
                {
                    _fileSystem.Directory.CreateDirectory(root);
                    _logger.LogInformation("Root application folder was created.", root, traceLevel);
                }

                if (_fileSystem.File.Exists(path))
                {
                    if (TimedAction.TryRun(async () =>
                    {
                        await Task.Run(() =>
                        {
                            var content = _fileSystem.File.ReadAllText(path);
                            if (!string.IsNullOrWhiteSpace(content))
                            {
                                var newValue = JsonSerializer.Deserialize<TSettings>(content, _options);
                                var newToken = Guid.NewGuid();
                                _tokens.Push(newToken);
                                TrySetValue(newValue, newToken);
                            }
                            else
                            {
                                _logger.LogWarning($"File was found, but no content was loaded.");
                            }
                        });
                    }, out var elapsed, out var exception))
                    {
                        _logger.LogPerformance("Deserialized settings from file.", elapsed, traceLevel);
                    }
                    else if (exception != default)
                    {
                        _logger.LogError(exception, traceLevel);
                    }
                }
                else
                {
                    _logger.LogWarning($"Settings file '{_path}' does not exist and will be created from defaults.");
                    WriteToFile();
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Error reading from file '{_path}'. Using cached/default settings.");
                throw;
            }

            return Value;
        }

        public void WriteToFile()
        {
            var traceLevel = ServerSettings.TraceLevel.ToTraceLevel();
            _logger.LogTrace("Writing to settings file...", _path, traceLevel);

                var settings = Value;
                var path = _path;
                var fileSystem = _fileSystem;

                if (TimedAction.TryRun(async () => {
                        var content = JsonSerializer.Serialize(settings.Settings);
                        await Task.Run(() => fileSystem.File.Create(path));
                    }, out var elapsed, out var exception))
                {
                    _logger.LogPerformance($"Serializing and writing content to file '{_path}' completed.", elapsed, traceLevel);
                }
                else if (exception != default)
                {
                    _logger.LogError(exception, traceLevel);
            }
        }
    }
}