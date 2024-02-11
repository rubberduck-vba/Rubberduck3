using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Rubberduck.InternalApi.Settings.Model;

namespace Rubberduck.Tests
{
    [TestClass]
    public abstract class ServiceBaseTest
    {
        protected IServiceProvider Services { get; private set; } = null!;
        protected Dictionary<Type, Mock> Mocks { get; private set; } = [];

        protected virtual IEnumerable<(Type, Mock)> ConfigureMocking() => [(typeof(IFileSystem), new Mock<IFileSystem>())];

        protected virtual void ConfigureServices(IServiceCollection services)
        {
            if (!Mocks.ContainsKey(typeof(IFileSystem)))
            {
                Mocks[typeof(IFileSystem)] = new Mock<IFileSystem>();
            }

            services.AddLogging();
            services.AddSingleton<ILogger>(provider => new TestLogger());
            services.AddSingleton(provider => (IFileSystem)Mocks[typeof(IFileSystem)].Object);
            services.AddSingleton<RubberduckSettingsProvider>();
            services.AddSingleton<IDefaultSettingsProvider<RubberduckSettings>>(provider => RubberduckSettings.Default);
            services.AddSingleton<PerformanceRecordAggregator>();
        }

        [TestInitialize]
        public virtual void OnInitialize()
        {
            var services = new ServiceCollection();
            Mocks = ConfigureMocking().ToDictionary();
            ConfigureServices(services);

            Services = services.BuildServiceProvider();
        }

        [TestCleanup]
        public virtual void OnCleanup()
        {
            Mocks.Clear();
            Mocks = null!;

            if (Services is IDisposable disposable)
            {
                disposable.Dispose();
            }
            Services = null!;
        }
    }
}
