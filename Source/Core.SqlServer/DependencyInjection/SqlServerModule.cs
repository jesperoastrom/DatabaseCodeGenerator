using Autofac;
using SqlFramework.Data;
using SqlFramework.Data.SqlServer2012.Extractors;

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