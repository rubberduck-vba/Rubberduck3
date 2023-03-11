using Microsoft.Extensions.DependencyInjection;
using Rubberduck.Resources.Registration;
using System.Security.Principal;
using System.Security.AccessControl;
using Microsoft.Extensions.Hosting;
using Rubberduck.DatabaseServer.Configuration;
using Rubberduck.ServerPlatform;
using System.Reflection;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging;

namespace Rubberduck.DatabaseServer
{
    internal static class Program
    {
        //private static StartupOptions _startupOptions;
        private static TimeSpan _exclusiveAccessTimeout = TimeSpan.FromSeconds(5);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static async Task<int> Main(string[] args)
        {
            #region Global Mutex https://stackoverflow.com/a/229567/1188513

            // get application GUID as defined in AssemblyInfo.cs
            var appGuid = RubberduckGuid.ServerGuid;

            // unique id for global mutex - Global prefix means it is global to the machine
            var mutexId = $"Global\\{{{appGuid}}}";

            // edited by Jeremy Wiebe to add example of setting up security for multi-user usage
            // edited by 'Marc' to work also on localized systems (don't use just "Everyone") 
            // edited by MasonGZhwiti to prevent race condition on security settings via VanNguyen
            using (var mutex = new Mutex(false, mutexId, out _))
            {
                var allowEveryoneRule = new MutexAccessRule(
                    new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                    MutexRights.FullControl,
                    AccessControlType.Allow);
                var securitySettings = new MutexSecurity();
                securitySettings.AddAccessRule(allowEveryoneRule);
                mutex.SetAccessControl(securitySettings);

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
                        throw;
                    }
                    /*
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
                    */
                    Console.WriteLine("Startup checks completed. Starting server application...");
                    try
                    {
                        //_startupOptions = startupArgs.Value;
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
            Console.WriteLine(ServerSplash.GetRenderString(Assembly.GetExecutingAssembly().GetName()));

            var tokenSource = new CancellationTokenSource();

            var builder = Host.CreateDefaultBuilder()
                .UseConsoleLifetime()
                .ConfigureLogging((context, logging) => 
                {
                    logging.AddConsole();
                })
                .ConfigureServices(provider => ConfigureServices(provider, tokenSource));

            var host = builder.Build();
            await host.StartAsync(tokenSource.Token);

            var canStart = false;

            try
            {
                var app = host.Services.GetRequiredService<Application>();
                await app.StartAsync();

                canStart = true;
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"FATAL: {exception}");
            }

            if (canStart)
            {
                await host.WaitForShutdownAsync();
            }
        }

        private static void ConfigureServices(IServiceCollection services, CancellationTokenSource tokenSource)
        {
            var config = LocalDbServerCapabilities.Default;
            services.AddRubberduckServerServices(config, tokenSource);
        }
    }
}