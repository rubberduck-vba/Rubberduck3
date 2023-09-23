using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Common;
using Rubberduck.SettingsProvider.Model;
using System;
using System.IO.Abstractions;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rubberduck.SettingsProvider
{
    public class SettingsService<TSettings>
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

        public TSettings Value
        {
            get => _cached;
            private set
            {
                _cached = value;
                _logger.LogTrace($"Cached new {typeof(TSettings)} value.");
            }
        }

        public async Task<TSettings> ReadFromFileAsync()
        {
            try
            {
                var elapsed = await TimedAction.RunAsync(Task.Run(() =>
                {
                    var content = _fileSystem.File.ReadAllText(_path);
                    Value = JsonSerializer.Deserialize<TSettings>(content, _options);
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
