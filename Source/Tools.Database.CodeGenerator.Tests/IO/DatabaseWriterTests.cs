using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using Autofac;
using Flip.Tools.Database.CodeGenerator.Configuration;
using Flip.Tools.Database.CodeGenerator.Data;
using Flip.Tools.Database.CodeGenerator.IO;
using Microsoft.CSharp;
using Microsoft.SqlServer.Management.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Smo = Microsoft.SqlServer.Management.Smo;



namespace Flip.Tools.Database.CodeGenerator.Tests.IO
{

	[TestClass]
	public sealed class DatabaseWriterTests
	{

		[TestInitialize]
		public void Initialize()
		{
			RunEmbeddedSetupScript(createScriptPath);
		}

		[TestCleanup]
		public void Cleanup()
		{
			RunEmbeddedSetupScript(deleteScriptPath);
		}

		[TestMethod]
		public void DatabaseWasCreated()
		{
			using (SqlConnection connection = new SqlConnection(testConnectionString))
			{
				connection.Open();
				using (SqlCommand command = new SqlCommand("select count(*) from Core.TestTable", connection))
				{
					command.CommandType = System.Data.CommandType.Text;
					using (SqlDataReader reader = command.ExecuteReader())
					{
						reader.Read();
					}
				}
			}
		}

		[TestMethod]
		public void CanWriteOutput()
		{
			IContainer container = CreateContainer(testConnectionString);

			var writer = container.Resolve<IDatabaseWriter>();
			var storageProvider = container.Resolve<IStorageProvider>() as MemoryStorageProvider;
			Assert.IsNotNull(storageProvider, "Memory storage provider is not used");


			string outputFile = "MyOutput";
			Assert.IsTrue(writer.WriteOutput(configurationPath, outputFile, "\t"));

			string code = storageProvider.GetOutputString(outputFile);

			CompilerResults results = CompileCode(code);
			Assert.AreEqual(0, results.Errors.Count, "Could not compile code:\n" + code);
		}



		private static void RunEmbeddedSetupScript(string resourcePath)
		{
			string script = EmbeddedResourceHelper.GetStringFromEmbeddedResource(resourcePath);

			using (SqlConnection connection = new SqlConnection(setupConnectionString))
			{
				var serverConnection = new ServerConnection(connection);
				var server = new Smo.Server(serverConnection);
				server.ConnectionContext.ExecuteNonQuery(script);
			}
		}

		private static CompilerResults CompileCode(string code)
		{
			var csc = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });
			var parameters = new CompilerParameters(new[] { "mscorlib.dll", "System.dll", "System.Core.dll", "System.Data.dll", "System.Xml.dll" }, "foo.exe", true);
			parameters.GenerateExecutable = true;
			CompilerResults results = csc.CompileAssemblyFromSource(parameters, code);
			foreach (CompilerError error in results.Errors)
			{
				Console.WriteLine(error.ErrorText);
			}
			Assert.AreEqual(0, results.Errors.Count, "Unable to compile code:\n" + code);
			return results;
		}

		private static IContainer CreateContainer(string connectionString)
		{
			var builder = new ContainerBuilder();

			builder.Register(c => new ConnectionStringProvider(connectionString)).As<IConnectionStringProvider>().SingleInstance();

			builder.RegisterCodeGeneratorTypes();

			builder.RegisterType<MemoryStorageProvider>().As<IStorageProvider>().SingleInstance();

			return builder.Build();
		}



		private static readonly string testConnectionString = ConfigurationManager.ConnectionStrings["test"].ConnectionString;
		private static readonly string setupConnectionString = ConfigurationManager.ConnectionStrings["setup"].ConnectionString;
		private const string createScriptPath = "Flip.Tools.Database.CodeGenerator.Tests.Resources.Sql.Create.Database.sql";
		private const string deleteScriptPath = "Flip.Tools.Database.CodeGenerator.Tests.Resources.Sql.Delete.Database.sql";
		private const string configurationPath = "Flip.Tools.Database.CodeGenerator.Tests.Resources.Configuration.Configuration.xml";
	}

}
