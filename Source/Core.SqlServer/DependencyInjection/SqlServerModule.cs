using Autofac;
using SqlFramework.Data;
using SqlFramework.Data.Extractors;
using SqlFramework.Data.Extractors.SqlServer;

namespace SqlFramework.DependencyInjection
{
    public sealed class SqlServerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DatabaseExtractor>().As<IDatabaseExtractor>().SingleInstance();
        }
    }
}