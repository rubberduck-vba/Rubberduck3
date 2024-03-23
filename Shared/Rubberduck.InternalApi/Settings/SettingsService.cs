using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings.Model;
using System;
using System.IO.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.Settings;

public class RubberduckSettingsProvider : SettingsService<RubberduckSettings>
{
    public RubberduckSettingsProvider(ILogger<SettingsService<RubberduckSettings>> logger,
        IFileSystem fileSystem,
        IDefaultSettingsProvider<RubberduckSettings> defaultSettings)
        : base(logger, null!, fileSystem, defaultSettings, null!)
    {
        SettingsProvider = this;
    }
}

public class SettingsService<TSettings> : ServiceBase, ISettingsService<TSettings>
    where TSettings : TypedSettingGroup, new()
{
    private static readonly JsonSerializerOptions _options = new()
    {
        IgnoreReadOnlyProperties = true,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement,
    };

    private readonly IFileSystem _fileSystem;
    private readonly TSettings _default;
    private readonly string _path;

    private TSettings _cached;

    public SettingsService(ILogger<SettingsService<TSettings>> logger,
        RubberduckSettingsProvider settings,
        IFileSystem fileSystem,
        IDefaultSettingsProvider<TSettings> defaultSettings,
        PerformanceRecordAggregator performance)
        : base(logger, settings, performance)
    {
        _fileSystem = fileSystem;

        var root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _path = _fileSystem.Path.Combine(root, "Rubberduck", $"{typeof(TSettings).Name}.json");

        _default = defaultSettings.Default;
        _cached = _default;
    }

    public event EventHandler<SettingsChangedEventArgs<TSettings>>? SettingsChanged;

    public void ClearCache()
    {
        _cached = _default;
        Read();
    }

    public new TSettings Settings => _cached;

    private bool TrySetValue(TSettings value)
    {
        /*TODO merge defaults*/
        _cached = value;
        return true;
    }


    public TSettings Read()
    {
        var path = _path;
        LogTrace("Reading settings from file...", path);

        try
        {
            if (_fileSystem.File.Exists(path))
            {
                if (TimedAction.TryRun(() =>
                {
                    var content = _fileSystem.File.ReadAllText(path);
                    LogTrace("File content successfully read from file.", $"File: '{path}'");

                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        var newValue = JsonSerializer.Deserialize<TSettings>(content, _options) ?? new();
                        LogTrace("File content successfully deserialized.");
                        TrySetValue(newValue);
                    }
                    else
                    {
                        LogWarning("File was found, but no content was loaded.");
                    }
                }, out var elapsed, out var exception))
                {
                    LogPerformance(elapsed, "Deserialized settings from file.");
                }
                else if (exception != default)
                {
                    LogException(exception);
                }
            }
            else
            {
                LogInformation("Settings file does not exist and will be created from defaults.", $"Path: '{path}'");
                Write(_default);
            }
        }
        catch (Exception exception)
        {
            LogWarning("Error reading from settings file. Cached settings remain in effect.", $"Path: '{_path}'");
            LogException(exception);
            throw;
        }

        return Settings;
    }

    public void Write(TSettings settings)
    {
        var traceLevel = TraceLevel;
        LogTrace("Writing to settings file...", _path);

        var path = _path;
        var fileSystem = _fileSystem;

        TryRunAction(() =>
        {
            var content = JsonSerializer.Serialize(settings, _options);
            fileSystem.File.WriteAllText(path, content);
        });

        _cached = settings;
    }

    void ISettingsChangedHandler<TSettings>.OnSettingsChanged(TSettings settings)
    {
        var oldValue = _cached;

        _cached = settings;
        Write(settings);

        SettingsChanged?.Invoke(this, new SettingsChangedEventArgs<TSettings>(oldValue, Settings));
    }
}