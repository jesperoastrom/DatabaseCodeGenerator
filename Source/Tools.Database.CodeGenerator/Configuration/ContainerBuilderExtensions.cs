using Autofac;
using Flip.Tools.Database.CodeGenerator.Data.Extractors;
using Flip.Tools.Database.CodeGenerator.IO;



namespace Flip.Tools.Database.CodeGenerator.Configuration
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
