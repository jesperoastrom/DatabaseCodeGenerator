using System;
using System.Collections.Generic;
using System.Linq;
using Flip.Tools.Database.CodeGenerator.Data.Models;
using Smo = Microsoft.SqlServer.Management.Smo;



namespace Flip.Tools.Database.CodeGenerator.Data.Extractors
{

	internal sealed class UserDefinedTableTypeExtractor
	{

		public UserDefinedTableTypeExtractor(Smo.UserDefinedTableTypeCollection tableTypes)
		{
			this.tableTypeLookup = tableTypes
				.Cast<Smo.UserDefinedTableType>()
				.ToDictionary(t => t.Schema.EscapeDatabaseName() + "." + t.Name.EscapeDatabaseName(), StringComparer.OrdinalIgnoreCase);
		}



		public SchemaCollection<UserDefinedTableTypeModel> Extract(Configuration.DatabaseConfiguration configuration)
		{
			var collection = new SchemaCollection<UserDefinedTableTypeModel>(configuration.UserDefinedTableTypes.Namespace);

			foreach (var element in configuration.UserDefinedTableTypes.Elements)
			{
				if (this.tableTypeLookup.ContainsKey(element.EscapedFullName))
				{
					Smo.UserDefinedTableType tableType = this.tableTypeLookup[element.EscapedFullName];
					collection.AddElement(tableType.Schema, ToModel(configuration.UserDefinedTableTypes.Namespace, tableType));
				}
				else
				{
					throw new ArgumentException("Unable to locate user defined type '" + element.EscapedFullName + "'");
				}
			}

			return collection;
		}



		private UserDefinedTableTypeModel ToModel(string typeNamespace, Smo.UserDefinedTableType tableType)
		{
			var model = new UserDefinedTableTypeModel();
			model.DatabaseName = new DatabaseName(tableType.Schema, tableType.Name);
			model.TypeName = new TypeName(typeNamespace, tableType.Name);
			model.Columns = ToModel(typeNamespace, tableType.Columns);
			return model;
		}

		private List<ColumnModel> ToModel(string typeNamespace, Smo.ColumnCollection columns)
		{
			return columns.Cast<Smo.Column>().Select(c => new ColumnModel()
			{
				DatabaseName = c.Name,
				ClrType = c.DataType.ToClrString(typeNamespace)
			}).ToList();
		}



		private readonly Dictionary<string, Smo.UserDefinedTableType> tableTypeLookup;

	}

}
