namespace SqlFramework.IO.Tests
{
    using System;
    using FluentAssertions;
    using Xunit;

    public sealed class DatabaseWriterTests : IDisposable
    {
        public DatabaseWriterTests()
        {
            TestScriptExecutor.RunEmbeddedScript(Properties.Resources.CreateDatabase);
        }

        [Fact]
        public void DatabaseWasCreated()
        {
            TestScriptExecutor.ExecuteReader("select count(*) from Core.LargeTable", reader => reader.Read().Should().BeTrue());
        }

        //[Fact(Skip = "Fix later")]
        //public void TestGenerateCode()
        //{
        //    IContainer container = CreateContainer(TestConnectionString);

        //    var builder = container.Resolve<IDatabaseWriter>();
        //    var storageProvider = container.Resolve<IStorageProvider>() as MemoryStorageProvider;
        //    storageProvider.Should().NotBeNull("Memory storage provider is not used");

        //    const string outputFile = "MyOutput";
        //    builder.WriteOutput(ConfigurationPath, outputFile, "\t").Should().BeTrue();

        //    string code = storageProvider.GetOutputString(outputFile);

        //    CompilerResults results = CompileCode(code);
        //    results.Errors.Count.Should().Be(0, "Could not compile code:\n" + code);

        //    Type proceduresWrapperType = results.CompiledAssembly.GetType("Database.Tests.StoredProcedures.Core");
        //    Type tableTypesWrapperType = results.CompiledAssembly.GetType("Database.Tests.UserDefinedTableTypes.Core");

        //    proceduresWrapperType.Should().NotBeNull("Unable to locate static type for stored procedures in Core schema");
        //    tableTypesWrapperType.Should().NotBeNull("Unable to locate static type for user defined table types in Core schema");
        //}

        //private void VerifyGetAllLargeTableItems(Type spStaticType)
        //{
        //    string spName = "GetAllLargeTableItems";
        //    Type procedureType = spStaticType.GetNestedType(spName);
        //    procedureType.Should().NotBeNull("Unable to locate stored procedure " + spStaticType.FullName + "." + spName);

        //    object sp = Activator.CreateInstance(procedureType);
        //    ExecuteResultFromConnectionString(procedureType, sp);
        //}

        void IDisposable.Dispose()
        {
            TestScriptExecutor.RunEmbeddedScript(Properties.Resources.DeleteDatabase);
        }

        //private static string AddMainMethod(string code)
        //{
        //    return code + "\nnamespace MainNamespace { class Program { static void Main(string[] args) { } } }";
        //}

        //private static CompilerResults CompileCode(string code)
        //{
        //    code = AddMainMethod(code);

        //    var codeProvider = new CSharpCodeProvider(new Dictionary<string, string>
        //                                                  {
        //                                                      { "CompilerVersion", "v4.0" }
        //                                                  });
        //    var parameters = new CompilerParameters(
        //        new[]
        //            {
        //                "mscorlib.dll",
        //                "System.dll",
        //                "System.Core.dll",
        //                "System.Data.dll",
        //                "System.Xml.dll"
        //            },
        //        "DatabaseTest.exe",
        //        true);

        //    parameters.GenerateExecutable = true;

        //    CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, code);
        //    foreach (CompilerError error in results.Errors)
        //    {
        //        Console.WriteLine(error.ErrorText);
        //    }
        //    results.Errors.Count.Should().Be(0, because: "Unable to compile code:\n" + code);
        //    return results;
        //}

        //private static IContainer CreateContainer(string connectionString)
        //{
        //    var builder = new ContainerBuilder();
        //    builder.Register(c => new ConnectionStringProvider(connectionString)).As<IConnectionStringProvider>().SingleInstance();
        //    builder.Register(c => new DebugLogger()).As<ILogger>().SingleInstance();
        //    builder.RegisterModule<CoreModule>();
        //    builder.RegisterType<MemoryStorageProvider>().As<IStorageProvider>().SingleInstance();
        //    return builder.Build();
        //    return null;
        //}

        //private static void ExecuteResultFromConnectionString(Type procedureType, object sp)
        //{
        //    MethodInfo method = procedureType.GetMethod("ExecuteResult", new[] { typeof(string) });
        //    method.Should().NotBeNull("Could not locate ExecuteResult method");

        //    method.Invoke(sp, new object[] { TestConnectionString.Value });
        //}

        //private static void HasExecuteResultFromCommand(Type procedureType, object sp)
        //{
        //    MethodInfo method = procedureType.GetMethod("ExecuteResult", new[] {typeof (SqlCommand)});
        //    Assert.IsNotNull(method, "Could not locate ExecuteResult method");
        //}        
    }
}