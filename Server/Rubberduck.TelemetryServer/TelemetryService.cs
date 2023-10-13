using Microsoft.Extensions.Logging;
using Rubberduck.ServerPlatform.Model.Telemetry;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.InternalApi.Extensions;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Rubberduck.InternalApi.Common;
using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Protocol;
using Rubberduck.InternalApi.Settings;

namespace Rubberduck.TelemetryServer
{

    public interface ITelemetryService
    {
        /// <summary>
        /// Enqueues the provided payload for later transmission.
        /// </summary>
        void Enqueue(TelemetryEventPayload payload);
        /// <summary>
        /// Gets all telemetry payloads enqueued for transmission.
        /// </summary>
        IEnumerable<TelemetryEventPayload> Payload { get; }
        /// <summary>
        /// Transmits all queued payloads to the Rubberduck web API.
        /// </summary>
        void Transmit();
        /// <summary>
        /// Clears the transmission queue.
        /// </summary>
        void Clear();
    }

    internal class TelemetryService : ITelemetryService
    {
        private readonly ILogger<TelemetryService> _logger;
        private readonly ISettingsProvider<TelemetryServerSettings> _settingsProvider;
        private readonly ConcurrentQueue<TelemetryEventPayload> _queue;
        private readonly ITelemetryTransmitter _transmitter;
        //private readonly Func<ILanguageServer> _server;

        private TraceLevel TraceLevel => _settingsProvider.Settings.TraceLevel.ToTraceLevel();

        public TelemetryService(ILogger<TelemetryService> logger, ISettingsProvider<TelemetryServerSettings> settingsProvider, 
            ITelemetryTransmitter transmitter/*, Func<ILanguageServer> server*/)
        {
            _logger = logger;
            _settingsProvider = settingsProvider;
            _transmitter = transmitter;
            _queue = new ConcurrentQueue<TelemetryEventPayload>();

            //_server = server;
        }

        public void Enqueue(TelemetryEventPayload payload)
        {
            _queue.Enqueue(payload);
            _logger.LogTrace(TraceLevel, "Enqueued telemetry payload", $"Type: {payload.GetType().Name} Queue length: {_queue.Count}");

            var settings = _settingsProvider.Settings;
            if (settings.StreamTransmission && _queue.Count >= settings.QueueSize)
            {
                _logger.LogInformation(TraceLevel, "TelemetryServerSettings.StreamTransmission is enabled; message queue has reached maximum configured queue size.", $"QueueSize setting: {settings.QueueSize}; Enqueued payloads: {_queue.Count}");
                Transmit();
            }
        }

        public IEnumerable<TelemetryEventPayload> Payload => _queue.AsEnumerable();

        public void Clear() => _queue.Clear();

        public void Transmit()
        {
            if (_queue.IsEmpty)
            {
                _logger.LogWarning(TraceLevel, "Telemetry payload queue is empty; nothing to transmit.");
                return;
            }

            var settings = _settingsProvider.Settings;
            //var server = _server();

            if (TimedAction.TryRun(() =>
            {
                var size = 0;
                var payload = new List<TelemetryEventPayload>();
                while (_queue.TryDequeue(out var telemetryItem))
                {
                    if (CanTransmitTelemetry(telemetryItem, settings))
                    {
                        payload.Add(telemetryItem);
                    }
                    size++;
                }

                _logger.LogInformation(TraceLevel, "Transmitting telemetry payload.", $"{payload.Count} events accepted; {size} events dequeued.");
                _transmitter.Transmit(payload);

            }, out var elapsed, out var exception))
            {
                _logger.LogPerformance(TraceLevel, "Transmitted telemetry payload.", elapsed);
            }
            else if (exception is not null)
            {
                _logger.LogError(TraceLevel, exception);
            }
        }

        private bool CanTransmitTelemetry(TelemetryEventPayload telemetryItem, TelemetryServerSettings settings)
        {
            if (settings.SendEventTelemetry && telemetryItem is EventTelemetry eventTelemetry)
            {
                if (!settings.EventTelemetryConfig.TryGetValue(eventTelemetry.Name, out var config))
                {
                    _logger.LogWarning(TraceLevel, $"EventTelemetry '{eventTelemetry.Name}' has no configuration and will be ignored.");
                }
                else
                {
                    if (config.IsEnabled)
                    {
                        _logger.LogTrace(TraceLevel, $"EventTelemetry '{eventTelemetry.Name}' payload is accepted.");
                    }

                    return config.IsEnabled;
                }
            }

            return false;
        }
    }
}
