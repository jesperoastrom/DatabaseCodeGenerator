namespace SqlFramework.DependencyInjection
{
    using Autofac;
    using Data.Extractors;
    using Data.Extractors.SqlServer;

    public sealed class SqlServerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DatabaseExtractor>().As<IDatabaseExtractor>().SingleInstance();
        }
    }
}