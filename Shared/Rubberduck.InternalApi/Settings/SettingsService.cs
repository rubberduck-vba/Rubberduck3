using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings.Model;
using System;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
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

    protected override RubberduckSettings MergeDefaults(RubberduckSettings value)
    {
        var result = value;
        var diff = RubberduckSettings.Default.Diff(value);
        foreach (var item in diff)
        {
            if (item.ReferenceValue is null)
            {
                // could be a deprecated setting that was un-shipped; remove this setting key.
                var newValue = value.TypedValue.Where(e => e.Key != item.Key).ToArray() ?? throw new InvalidOperationException();
                result = (RubberduckSettings)value.WithValue(newValue);
                LogInformation($"Setting key '{item.Key}' was removed.");
            }
            else if (item.ComparableValue is null)
            {
                // could be a new setting that wasn't in the version settings were created with; add this setting key.
                var newValue = value.WithSetting(item.ReferenceValue);
                result = (RubberduckSettings)value.WithValue(newValue);
                LogInformation($"Setting key '{item.Key}' was created with default value '{item.ReferenceValue.Value}'.");
            }
            else
            {
                // otherwise just take the value from the settings file (comparable).
                // ...and since we've initialized result with the incoming value, we've already done that. right?
                var newValue = value.WithSetting(item.ComparableValue);
                result = (RubberduckSettings)value.WithSetting(newValue);
                LogInformation($"Setting key '{item.Key}' has a non-default configuration.", $"Value: {item.ComparableValue.Value} | Default: {item.ReferenceValue.Value}");
            }
        }

        return result;
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

                        var mergedValue = MergeDefaults(newValue);
                        TrySetValue(mergedValue);
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

    protected virtual TSettings MergeDefaults(TSettings value) => throw new NotImplementedException();

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