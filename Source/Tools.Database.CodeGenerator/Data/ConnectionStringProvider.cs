﻿namespace Flip.Tools.Database.CodeGenerator.Data
{

	public sealed class ConnectionStringProvider : IConnectionStringProvider
	{

		public ConnectionStringProvider(string connectionString)
		{
			this.ConnectionString = connectionString;
		}

		public string ConnectionString{get; private set;}

	}

}
