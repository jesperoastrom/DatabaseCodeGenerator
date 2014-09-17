namespace SqlFramework.DependencyInjection
{
    using Autofac;
    using Configuration;
    using IO.OutputDestinations;
    using IO.StorageProviders;
    using IO.Writers;

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