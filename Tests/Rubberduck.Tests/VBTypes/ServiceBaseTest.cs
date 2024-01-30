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

namespace Rubberduck.Tests.VBTypes
{
    [TestClass]
    internal abstract class ServiceBaseTest<SUT> where SUT : class
    {
        protected IServiceProvider Services { get; private set; } = null!;
        protected Dictionary<Type, Mock> Mocks { get; private set; } = null!;

        protected virtual IEnumerable<(Type, Mock)> ConfigureMocking() => [(typeof(IFileSystem), new Mock<IFileSystem>())];

        protected virtual void ConfigureServices(IServiceCollection services)
        {
            if (!Mocks.ContainsKey(typeof(IFileSystem)))
            {
                Mocks[(typeof(IFileSystem))] = new Mock<IFileSystem>();
            }

            services.AddSingleton<SUT>();
            services.AddSingleton<ILogger>(provider => new TestLogger());
            services.AddSingleton<IFileSystem>(provider => (IFileSystem)Mocks[typeof(IFileSystem)].Object);
            services.AddSingleton<RubberduckSettingsProvider>();
            services.AddSingleton<IDefaultSettingsProvider<RubberduckSettings>>(provider => RubberduckSettings.Default);
            services.AddSingleton<PerformanceRecordAggregator>();
        }

        protected virtual SUT CreateSUT() => Services.GetRequiredService<SUT>();

        [TestInitialize]
        public virtual void OnInitialize()
        {
            var services = new ServiceCollection();
            ConfigureMocking();
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
