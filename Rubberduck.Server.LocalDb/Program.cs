using CommandLine;
using Rubberduck.InternalApi.RPC;
using Rubberduck.RPC.Platform;
using Rubberduck.Server.LocalDb.Properties;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace Rubberduck.Server.LocalDb
{
    internal class Program
    {
        private static TimeSpan _exclusiveAccessTimeout = TimeSpan.FromSeconds(5);
        private static ManualResetEvent _shutdownSignal = new ManualResetEvent(false);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            #region Global Mutex https://stackoverflow.com/a/229567/1188513

            // get application GUID as defined in AssemblyInfo.cs
            var appGuid = ((GuidAttribute)Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(GuidAttribute), false)
                .GetValue(0)).Value
                .ToString();

            // unique id for global mutex - Global prefix means it is global to the machine
            var mutexId = $"Global\\{{{appGuid}}}";

            // edited by Jeremy Wiebe to add example of setting up security for multi-user usage
            // edited by 'Marc' to work also on localized systems (don't use just "Everyone") 
            var allowEveryoneRule = new MutexAccessRule(
                new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                MutexRights.FullControl,
                AccessControlType.Allow);
            var securitySettings = new MutexSecurity();
            securitySettings.AddAccessRule(allowEveryoneRule);

            // edited by MasonGZhwiti to prevent race condition on security settings via VanNguyen
            using (var mutex = new Mutex(false, mutexId, out _, securitySettings))
            {
                // edited by acidzombie24
                var hasHandle = false;
                try
                {
                    try
                    {
                        // edited by acidzombie24
                        hasHandle = mutex.WaitOne(_exclusiveAccessTimeout.Milliseconds, false);
                        if (!hasHandle) throw new TimeoutException("Timeout waiting for exclusive access");
                    }
                    catch (AbandonedMutexException)
                    {
                        // Log the fact that the mutex was abandoned in another process,
                        // it will still get acquired
                        hasHandle = true;
                    }

                    var startupArgs = Parser.Default.ParseArguments<StartupOptions>(args)
                        .WithNotParsed(errors =>
                        {
                            Console.WriteLine("Errors have occurred processing command-line arguments. Server will not be started.");
                        })
                        .WithParsed(options =>
                        {
                            Console.WriteLine("Command-line arguments successfully parsed. Proceeding with startup...");
                        });

                    if (startupArgs.Errors.Any())
                    {
                        throw new ArgumentException("Invalid command-line arguments were supplied.");
                    }

                    Console.WriteLine("Startup checks completed. Starting server application...");
                    Start(startupArgs.Value);
                }
                finally
                {
                    // edited by acidzombie24, added if statement
                    if (hasHandle)
                    {
                        mutex.ReleaseMutex();
                    }
                }
            }
            #endregion
        }

        private static void Start(StartupOptions startupOptions)
        {
            ValidatePort(startupOptions.Port);

            var console = new JsonRpcConsole
            {
                IsEnabled = !startupOptions.Silent,
                Trace = startupOptions.Silent ? Constants.TraceValue.Off
                    : startupOptions.Verbose
                        ? Constants.TraceValue.Verbose
                        : Constants.TraceValue.Messages
            };

            var server = new LocalDbServer(Settings.Default.JsonRpcServerPath, startupOptions.Port, console);
            server.ShutdownSignal += (o, e) => _shutdownSignal.Set();
            server.Start();

            _shutdownSignal.WaitOne();
        }

        private static void ValidatePort(int port)
        {
            if (!RpcPort.IsValid(port))
            {
                throw new ArgumentOutOfRangeException("Invalid RPC port. Must be in the range 1024-5000.");
            }
        }
    }
}
