using Autofac;
using SqlFramework.Data.Extractors;
using SqlFramework.IO;



namespace SqlFramework.Configuration
{

	public static class ContainerBuilderExtensions
	{

		public static void RegisterCodeGeneratorTypes(this ContainerBuilder builder)
		{

			builder.RegisterType<ConsoleTextWriter>().As<ITextWriter>().SingleInstance();

			builder.RegisterType<ConfigurationReader>().As<IConfigurationReader>().SingleInstance();

			builder.RegisterType<DatabaseExtractor>().As<IDatabaseExtractor>().SingleInstance();

			builder.RegisterType<FileStorageProvider>().As<IStorageProvider>().SingleInstance();

			builder.RegisterType<DatabaseWriter>().As<IDatabaseWriter>().SingleInstance();

		}

	}

}
