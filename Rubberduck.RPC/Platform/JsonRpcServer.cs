using System;
using System.Diagnostics;
using System.Linq;
using NLog;
using Rubberduck.InternalApi.Common;
using WebSocketSharp.Server;

namespace Rubberduck.RPC.Platform
{
    public interface IJsonRpcServer
    {
        IJsonRpcConsole Console { get; }

        int ProcessId { get; }
        int Port { get; }
        string Path { get; }

        bool IsAlive { get; }
        TimeSpan Uptime { get; }
        DateTime? SessionStart { get; }

        int MessagesReceived { get; }
        int MessagesSent { get; }

        void Start();
        void Stop();
    }

    public abstract class JsonRpcServer : IJsonRpcServer
    {
        private readonly WebSocketServer _socketServer;
        private Stopwatch _uptimeStopwatch = new Stopwatch();

        protected JsonRpcServer(string path, int port, IJsonRpcConsole console)
        {
            _socketServer = new WebSocketServer(port);
            _socketServer.KeepClean = false;

            ProcessId = Process.GetCurrentProcess().Id;

            Path = path;
            Port = port;

            Console = console;
        }

        /// <summary>
        /// Gets the ID of the server process.
        /// </summary>
        public int ProcessId { get; }

        /// <summary>
        /// The session start timestamp.
        /// </summary>
        public DateTime? SessionStart { get; private set; }

        /// <summary>
        /// The current trace level for this server.
        /// </summary>
        public string Trace { get; set; }

        /// <summary>
        /// An output console for this server.
        /// </summary>
        public IJsonRpcConsole Console { get; }

        /// <summary>
        /// <c>true</c> if the socket server is listening to its assigned port.
        /// </summary>
        public bool IsAlive => _socketServer.IsListening;

        /// <summary>
        /// <c>true</c> if this server can display an interactive user interface.
        /// </summary>
        public bool IsInteractive { get; }

        /// <summary>
        /// Gets the RPC port this server is configured with.
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Gets the JSON-RPC path for this server.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// The number of requests received by this server.
        /// </summary>
        public int MessagesReceived { get; private set; }

        /// <summary>
        /// The number of responses returned by this server.
        /// </summary>
        /// <remarks>
        /// Notification requests (LSP) do not get a response.
        /// </remarks>
        public int MessagesSent { get; private set; }

        /// <summary>
        /// Gets a <c>TimeSpan</c> representing the time elapsed since the server was started.
        /// </summary>
        public TimeSpan Uptime => _uptimeStopwatch.Elapsed;

        private void ConfigureServices()
        {
            _socketServer.AddWebSocketService<JsonRpcBehavior>(Path, InitializeRpc);
        }

        private void InitializeRpc(JsonRpcBehavior behavior)
        {
            behavior.WebSocketOpened += OnWebSocketOpened;
            behavior.MessageReceived += OnWebSocketMessage;
            behavior.MessageSent += OnWebSocketMessageSent;
            behavior.WebSocketClosed += OnWebSocketClosed;
        }

        private void OnWebSocketMessageSent(object sender, OutgoingMessageEventArgs e)
        {
            MessagesSent++;
            Console.Log(LogLevel.Trace, "Response message sent.", verbose: e.Response);
        }

        private void OnWebSocketMessage(object sender, WebSocketSharp.MessageEventArgs e)
        {
            if (e.IsPing)
            {
                Console.Log(LogLevel.Trace, "Ping message received.");
            }
            else if (e.IsText)
            {
                MessagesReceived++;
                Console.Log(LogLevel.Trace, "Request message received.", verbose: e.Data);
            }
        }

        private void OnWebSocketClosed(object sender, WebSocketSharp.CloseEventArgs e)
        {
            Console.Log(LogLevel.Info, "Socket connection closed.", verbose: $"Code: {e.Code} Reason: {e.Reason}");
            if (!e.WasClean)
            {
                Console.Log(LogLevel.Warn, $"Socket connection was not cleanly closed.");
            }
        }

        private void OnWebSocketOpened(object sender, EventArgs e)
        {
            Console.Log(LogLevel.Info, "Socket connection opened.", verbose: $"Port: {_socketServer.Port}");
        }

        public void Start()
        {
            Console.Log(LogLevel.Info, $"Starting server...");
            var elapsed = TimedAction.Run(() =>
            {
                if (!_socketServer.IsListening)
                {
                    _socketServer.Start();
                    SessionStart = DateTime.Now;
                    Console.Log(LogLevel.Trace, $"Socket server started.", verbose: $"Port: {_socketServer.Port} SessionStart: {SessionStart:o}");

                    _uptimeStopwatch.Restart();
                    Console.Log(LogLevel.Trace, $"Uptime stopwatch started.");

                    ConfigureServices();
                    Console.Log(LogLevel.Trace, $"JsonRpc services configured.", verbose: $"Path(s): {string.Join(";", _socketServer.WebSocketServices.Paths.Select(p => $"'{p}'"))}");
                }
                else
                {
                    Console.Log(LogLevel.Debug, $"{nameof(JsonRpcClient)}.{nameof(Start)}() was called, but server is already listening on port {_socketServer.Port}.");
                }
            });
            Console.Log(LogLevel.Trace, $"{nameof(JsonRpcClient)}.{nameof(Start)}() completed in {elapsed.TotalMilliseconds:N0}ms.");
        }

        public void Stop()
        {
            Console.Log(LogLevel.Info, $"Stopping server...");
            var elapsed = TimedAction.Run(() =>
            {
                if (_socketServer.IsListening)
                {
                    _socketServer.Stop();
                    Console.Log(LogLevel.Info, $"Socket server stopped.");

                    _uptimeStopwatch.Stop();
                    Console.Log(LogLevel.Debug, $"Uptime stopwatch stopped at {_uptimeStopwatch.Elapsed:g}.");
                }
                else
                {
                    Console.Log(LogLevel.Debug, $"{nameof(JsonRpcClient)}.{nameof(Stop)}() was called, but server has already stopped listening.");
                }
            });
            Console.Log(LogLevel.Trace, $"{nameof(JsonRpcClient)}.{nameof(Stop)}() completed in {elapsed.TotalMilliseconds:N0}ms.");
        }
    }
}
