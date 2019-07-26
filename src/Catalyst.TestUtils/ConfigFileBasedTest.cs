#region LICENSE

/**
* Copyright (c) 2019 Catalyst Network
*
* This file is part of Catalyst.Node <https://github.com/catalyst-network/Catalyst.Node>
*
* Catalyst.Node is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 2 of the License, or
* (at your option) any later version.
*
* Catalyst.Node is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with Catalyst.Node. If not, see <https://www.gnu.org/licenses/>.
*/

#endregion

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac;
using Autofac.Configuration;
using AutofacSerilogIntegration;
using Catalyst.Common.Config;
using Catalyst.Common.Interfaces.Cryptography;
using Catalyst.Common.Interfaces.FileSystem;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using SharpRepository.Ioc.Autofac;
using SharpRepository.Repository;
using Xunit.Abstractions;

namespace Catalyst.TestUtils
{
    public abstract class ConfigFileBasedTest : FileSystemBasedTest
    {
        protected ContainerBuilder ContainerBuilder;
        private IConfigurationRoot _configRoot;
        protected virtual bool WriteLogsToTestOutput => false;
        protected virtual bool WriteLogsToFile => false;
        protected ConfigFileBasedTest(ITestOutputHelper output) : base(output) { }

        protected string LogOutputTemplate { get; set; } =
            "{Timestamp:HH:mm:ss} [{Level:u3}] ({ThreadId}) {Message} ({SourceContext}){NewLine}{Exception}";

        protected LogEventLevel LogEventLevel { get; set; } = LogEventLevel.Verbose;

        protected abstract IEnumerable<string> ConfigFilesUsed { get; }

        protected IConfigurationRoot ConfigurationRoot
        {
            get
            {
                if (_configRoot != null)
                {
                    return _configRoot;
                }

                var configBuilder = new ConfigurationBuilder();
                ConfigFilesUsed.ToList().ForEach(f => configBuilder.AddJsonFile(f));

                _configRoot = configBuilder.Build();
                return _configRoot;
            }
        }

        protected virtual void ConfigureContainerBuilder()
        {
            var configurationModule = new ConfigurationModule(ConfigurationRoot);
            ContainerBuilder = new ContainerBuilder();
            ContainerBuilder.RegisterModule(configurationModule);
            ContainerBuilder.RegisterInstance(ConfigurationRoot).As<IConfigurationRoot>();

            var repoFactory =
                RepositoryFactory.BuildSharpRepositoryConfiguation(ConfigurationRoot.GetSection("CatalystNodeConfiguration:PersistenceConfiguration"));
            ContainerBuilder.RegisterSharpRepository(repoFactory);

            var passwordReader = new TestPasswordReader();
            ContainerBuilder.RegisterInstance(passwordReader).As<IPasswordReader>();

            var certificateStore = new TestCertificateStore();
            ContainerBuilder.RegisterInstance(certificateStore).As<ICertificateStore>();
            ContainerBuilder.RegisterInstance(FileSystem).As<IFileSystem>();

            ConfigureLogging();
        }

        private void ConfigureLogging()
        {
            var loggerConfiguration = new LoggerConfiguration().ReadFrom.Configuration(ConfigurationRoot).MinimumLevel.Verbose();
            
            if (WriteLogsToTestOutput)
            {
                loggerConfiguration.WriteTo.TestOutput(Output, LogEventLevel, LogOutputTemplate);
            }

            if (WriteLogsToFile)
            {
                loggerConfiguration.WriteTo.File(Path.Combine(FileSystem.GetCatalystDataDir().FullName, "Catalyst.Node.log"), LogEventLevel,
                    LogOutputTemplate);
            }

            ContainerBuilder.RegisterLogger(loggerConfiguration.CreateLogger());
        }
    }
}
