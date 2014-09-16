using Autofac;
using SqlFramework.Configuration;
using SqlFramework.IO;
using SqlFramework.IO.OutputDestinations;
using SqlFramework.IO.StorageProviders;
using SqlFramework.IO.Writers;

namespace SqlFramework.DependencyInjection
{
    public sealed class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ConsoleOutputDestination>().As<IOutputDestination>().SingleInstance();
            builder.RegisterType<ConfigurationReader>().As<IConfigurationReader>().SingleInstance();
            builder.RegisterType<FileStorageProvider>().As<IStorageProvider>().SingleInstance();
            builder.RegisterType<DatabaseWriter>().As<IDatabaseWriter>().SingleInstance();
        }
    }
}