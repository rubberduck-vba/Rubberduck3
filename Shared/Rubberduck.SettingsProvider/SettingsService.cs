using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Common;
using Rubberduck.SettingsProvider.Model;
using System;
using System.IO.Abstractions;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rubberduck.SettingsProvider
{
    public interface ISettingsService<TSettings> : ISettingsProvider<TSettings>
    {
        event EventHandler<SettingsChangedEventArgs<TSettings>> SettingsChanged;

        Task<TSettings> ReadFromFileAsync();
        Task WriteToFileAsync(TSettings settings);
    }

    public class SettingsService<TSettings> : ISettingsService<TSettings>
    {
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;
        private readonly TSettings _default;

        private readonly string _path;

        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        private TSettings _cached;

        public SettingsService(ILogger<SettingsService<TSettings>> logger, IFileSystem fileSystem, IDefaultSettingsProvider<TSettings> defaultSettings) 
        {
            _logger = logger;
            _fileSystem = fileSystem;
            _default = defaultSettings.Default;
            _cached = _default;

            _path = _fileSystem.Path.Combine($"{typeof(TSettings).Name}.json"); // TODO put the %userappdata% special folder in there
        }

        public event EventHandler<SettingsChangedEventArgs<TSettings>> SettingsChanged;

        private void OnSettingsChanged(TSettings oldValue)
        {
            var args = new SettingsChangedEventArgs<TSettings>(oldValue, _cached);
            SettingsChanged?.Invoke(this, args);
        }

        public TSettings Value
        {
            get => _cached;
            private set
            {
                if (!_cached.Equals(value))
                {
                    _cached = value;
                    _logger.LogTrace($"Cached new {typeof(TSettings).Name} value.");
                    OnSettingsChanged(value);
                }
                else
                {
                    _logger.LogTrace($"{typeof(TSettings).Name} value is unchanged.");
                }
            }
        }

        public async Task<TSettings> ReadFromFileAsync()
        {
            try
            {
                var elapsed = await TimedAction.RunAsync(Task.Run(() =>
                {
                    var content = _fileSystem.File.ReadAllText(_path);
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        Value = JsonSerializer.Deserialize<TSettings>(content, _options);
                    }
                    else
                    {
                        _logger.LogWarning($"File was found, but no content was loaded.");
                    }
                }));
                _logger.LogTrace($"PERF: Reading and deserializing file '{_path}' took {elapsed.TotalMilliseconds}ms.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error reading from file '{_path}'. Returning cached/default settings.");
            }

            return Value;
        }

        public async Task WriteToFileAsync(TSettings settings)
        {
            try
            {
                Value = settings;
                var elapsed = await TimedAction.RunAsync(Task.Run(() =>
                {
                    var content = JsonSerializer.Serialize(settings);
                    _fileSystem.File.Create(_path);
                }));
                _logger.LogTrace($"PERF: Serializing and writing content to file '{_path}' took {elapsed.TotalMilliseconds}ms.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error writing to file '{_path}'.");

            }
        }
    }
}