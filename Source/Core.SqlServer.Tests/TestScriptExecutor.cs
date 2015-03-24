namespace SqlFramework
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Microsoft.SqlServer.Management.Common;
    using Microsoft.SqlServer.Management.Smo;

    public static class TestScriptExecutor
    {
        public static void RunEmbeddedScript(string script)
        {
            using (var connection = new SqlConnection(TestConnectionString.Value))
            {
                var serverConnection = new ServerConnection(connection);
                var server = new Server(serverConnection);
                server.ConnectionContext.ExecuteNonQuery(script);
            }
        }

        public static void ExecuteReader(string script, Action<SqlDataReader> read)
        {
            using (var connection = new SqlConnection(TestConnectionString.Value))
            {
                connection.Open();
                using (var command = new SqlCommand(script, connection))
                {
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        read(reader);
                    }
                }
            }
        }
    }
}
