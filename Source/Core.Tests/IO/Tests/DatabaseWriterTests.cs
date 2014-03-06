using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Autofac;
using FluentAssertions;
using Microsoft.CSharp;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using SqlFramework.Data;
using SqlFramework.DependencyInjection;
using SqlFramework.Logging;
using Xunit;

namespace SqlFramework.IO.Tests
{
    public sealed class DatabaseWriterTests : IDisposable
    {
        public DatabaseWriterTests()
        {
            RunEmbeddedSetupScript(CreateScriptPath);
        }

        [Fact]
        public void DatabaseWasCreated()
        {
            using (var connection = new SqlConnection(TestConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("select count(*) from Core.LargeTable", connection))
                {
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                    }
                }
            }
        }

        [Fact(Skip = "Fix later")]
        public void TestGenerateCode()
        {
            //IContainer container = CreateContainer(TestConnectionString);

            //var writer = container.Resolve<IDatabaseWriter>();
            //var storageProvider = container.Resolve<IStorageProvider>() as MemoryStorageProvider;
            //storageProvider.Should().NotBeNull("Memory storage provider is not used");

            //const string outputFile = "MyOutput";
            //writer.WriteOutput(ConfigurationPath, outputFile, "\t").Should().BeTrue();

            //string code = storageProvider.GetOutputString(outputFile);

            //CompilerResults results = CompileCode(code);
            //results.Errors.Count.Should().Be(0, "Could not compile code:\n" + code);

            //Type proceduresWrapperType = results.CompiledAssembly.GetType("Database.Tests.StoredProcedures.Core");
            //Type tableTypesWrapperType = results.CompiledAssembly.GetType("Database.Tests.UserDefinedTableTypes.Core");

            //proceduresWrapperType.Should().NotBeNull("Unable to locate static type for stored procedures in Core schema");
            //tableTypesWrapperType.Should().NotBeNull("Unable to locate static type for user defined table types in Core schema");
        }

        //private void VerifyGetAllLargeTableItems(Type spStaticType)
        //{
        //    string spName = "GetAllLargeTableItems";
        //    Type spType = spStaticType.GetNestedType(spName);
        //    spType.Should().NotBeNull("Unable to locate stored procedure " + spStaticType.FullName + "." + spName);

        //    object sp = Activator.CreateInstance(spType);
        //    ExecuteResultFromConnectionString(spType, sp);
        //}

        void IDisposable.Dispose()
        {
            RunEmbeddedSetupScript(DeleteScriptPath);
        }

        private static string AddMainMethod(string code)
        {
            return code + "\nnamespace MainNamespace { class Program { static void Main(string[] args) { } } }";
        }

        private static CompilerResults CompileCode(string code)
        {
            code = AddMainMethod(code);

            var codeProvider = new CSharpCodeProvider(new Dictionary<string, string>
                                                          {
                                                              {"CompilerVersion", "v4.0"}
                                                          });
            var parameters = new CompilerParameters(new[]
                                                        {
                                                            "mscorlib.dll",
                                                            "System.dll",
                                                            "System.Core.dll",
                                                            "System.Data.dll",
                                                            "System.Xml.dll"
                                                        }, "DatabaseTest.exe", true) {GenerateExecutable = true};
            CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, code);
            foreach (CompilerError error in results.Errors)
            {
                Console.WriteLine(error.ErrorText);
            }
            results.Errors.Count.Should().Be(0, reason: "Unable to compile code:\n" + code);
            return results;
        }

        private static IContainer CreateContainer(string connectionString)
        {
            //var builder = new ContainerBuilder();
            //builder.Register(c => new ConnectionStringProvider(connectionString)).As<IConnectionStringProvider>().SingleInstance();
            //builder.Register(c => new DebugLogger()).As<ILogger>().SingleInstance();
            //builder.RegisterModule<CoreModule>();
            //builder.RegisterType<MemoryStorageProvider>().As<IStorageProvider>().SingleInstance();
            //return builder.Build();
            return null;
        }

        private static void ExecuteResultFromConnectionString(Type spType, object sp)
        {
            MethodInfo method = spType.GetMethod("ExecuteResult", new[] {typeof (string)});
            method.Should().NotBeNull("Could not locate ExecuteResult method");

            method.Invoke(sp, new object[] {TestConnectionString});
        }

        //private static void HasExecuteResultFromCommand(Type spType, object sp)
        //{
        //    MethodInfo method = spType.GetMethod("ExecuteResult", new[] {typeof (SqlCommand)});
        //    Assert.IsNotNull(method, "Could not locate ExecuteResult method");
        //}

        private static void RunEmbeddedSetupScript(string resourcePath)
        {
            string script = EmbeddedResourceHelper.GetStringFromEmbeddedResource(resourcePath);

            using (var connection = new SqlConnection(SetupConnectionString))
            {
                var serverConnection = new ServerConnection(connection);
                var server = new Server(serverConnection);
                server.ConnectionContext.ExecuteNonQuery(script);
            }
        }

        private const string CreateScriptPath = "SqlFramework.Resources.Tests.Sql.Create.Database.sql";
        private const string DeleteScriptPath = "SqlFramework.Resources.Tests.Sql.Delete.Database.sql";
        private const string ConfigurationPath = "SqlFramework.Resources.Tests.Configuration.Configuration.xml";
        private static readonly string TestConnectionString = ConfigurationManager.ConnectionStrings["test"].ConnectionString;
        private static readonly string SetupConnectionString = ConfigurationManager.ConnectionStrings["setup"].ConnectionString;
    }
}