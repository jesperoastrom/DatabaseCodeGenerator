using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Reflection;
using Autofac;
using Flip.Tools.Database.CodeGenerator.Configuration;
using Flip.Tools.Database.CodeGenerator.Data;
using Flip.Tools.Database.CodeGenerator.IO;
using Microsoft.CSharp;
using Microsoft.SqlServer.Management.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Smo = Microsoft.SqlServer.Management.Smo;
using Flip.Tools.Database.CodeGenerator.Tests.Logging;



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
				using (SqlCommand command = new SqlCommand("select count(*) from Core.LargeTable", connection))
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
		public void TestGenerateCode()
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

			Type proceduresWrapperType = results.CompiledAssembly.GetType("Database.Tests.StoredProcedures.Core");
			Type tableTypesWrapperType = results.CompiledAssembly.GetType("Database.Tests.UserDefinedTableTypes.Core");

			Assert.IsNotNull(proceduresWrapperType, "Unable to locate static type for stored procedures in Core schema");
			Assert.IsNotNull(tableTypesWrapperType, "Unable to locate static type for user defined table types in Core schema");
		}

		private void VerifyGetAllLargeTableItems(Type spStaticType)
		{
			string spName = "GetAllLargeTableItems";
			Type spType = spStaticType.GetNestedType(spName);
			Assert.IsNotNull(spType, "Unable to locate stored procedure " + spStaticType.FullName + "." + spName);

			object sp = Activator.CreateInstance(spType);
			ExecuteResultFromConnectionString(spType, sp);
		}

		private static object ExecuteResultFromConnectionString(Type spType, object sp)
		{
			MethodInfo method = spType.GetMethod("ExecuteResult", new Type[] { typeof(string) });
			Assert.IsNotNull(method, "Could not locate ExecuteResult method");

			return method.Invoke(sp, new object[] { testConnectionString });
		}

		private static void HasExecuteResultFromCommand(Type spType, object sp)
		{
			MethodInfo method = spType.GetMethod("ExecuteResult", new Type[] { typeof(SqlCommand) });
			Assert.IsNotNull(method, "Could not locate ExecuteResult method");
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
			code = AddMainMethod(code);

			var codeProvider = new CSharpCodeProvider(new Dictionary<string, string>() 
			{ 
				{ "CompilerVersion", "v4.0" }
			});
			var parameters = new CompilerParameters(new[] 
			{ 
				"mscorlib.dll", 
				"System.dll", 
				"System.Core.dll", 
				"System.Data.dll", 
				"System.Xml.dll" 
			}, "DatabaseTest.exe", true);
			parameters.GenerateExecutable = true;
			CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, code);
			foreach (CompilerError error in results.Errors)
			{
				Console.WriteLine(error.ErrorText);
			}
			Assert.AreEqual(0, results.Errors.Count, "Unable to compile code:\n" + code);
			return results;
		}

		private static string AddMainMethod(string code)
		{
			return code + "\nnamespace MainNamespace { class Program { static void Main(string[] args) { } } }";
		}

		private static IContainer CreateContainer(string connectionString)
		{
			var builder = new ContainerBuilder();

			builder.Register(c => new ConnectionStringProvider(connectionString)).As<IConnectionStringProvider>().SingleInstance();
			builder.Register(c => new DebugLogger()).As<ILogger>().SingleInstance();

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
