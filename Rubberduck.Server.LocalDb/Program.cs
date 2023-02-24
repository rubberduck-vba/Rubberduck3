using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rubberduck.Server.LocalDb.Configuration;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Server.LocalDb
{
    internal static class Program
    {
        private static StartupOptions _startupOptions;
        private static TimeSpan _exclusiveAccessTimeout = TimeSpan.FromSeconds(5);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static async Task<int> Main(string[] args)
        {
            var context = SynchronizationContext.Current;

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
                        // something went wrong
                        Thread.CurrentThread.Abort();
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
                    try
                    {
                        _startupOptions = startupArgs.Value;
                        await StartAsync();
                    }
                    catch (OperationCanceledException)
                    {
                        // normal exit
                        return 0;
                    }
                    catch (Exception exception)
                    {
                        // any other exception type exits with an error code
                        await Console.Error.WriteLineAsync(exception.ToString());
                        return 1;
                    }
                }
                finally
                {
                    // edited by acidzombie24, added if statement
                    if (hasHandle)
                    {
                        // MG: can only do this from the main thread...
                        if (!Thread.CurrentThread.IsBackground)
                        {
                            mutex.ReleaseMutex(); // FIXME does not get called
                        }
                        else
                        {
                            // mutex gets released on dispose, I guess
                        }
                    }
                }

                // normal exit // FIXME would be nice to be on the main thread here, but we're not
                return 0;
            }
            #endregion
        }

        private static async Task StartAsync()
        {
            var tokenSource = new CancellationTokenSource();

            var builder = Host.CreateDefaultBuilder()
                .UseConsoleLifetime()
                .ConfigureServices(provider => ConfigureServices(provider, tokenSource));

            await builder.RunConsoleAsync(tokenSource.Token);
        }

        private static void ConfigureServices(IServiceCollection services, CancellationTokenSource cts)
        {
            var config = LocalDbServerCapabilities.Default;
            services.AddRubberduckServerServices(config, cts);
        }
    }
}
