﻿using System;
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



		public SchemaCollection<UserDefinedTableTypeModel> Extract(Configuration.UserDefinedTableTypes userDefinedTableTypes)
		{
			var collection = new SchemaCollection<UserDefinedTableTypeModel>(userDefinedTableTypes.Namespace);

			foreach (var element in userDefinedTableTypes.Elements)
			{
				if (this.tableTypeLookup.ContainsKey(element.EscapedFullName))
				{
					Smo.UserDefinedTableType tableType = this.tableTypeLookup[element.EscapedFullName];
					collection.AddElement(tableType.Schema, ToModel(tableType));
				}
				else
				{
					throw new ArgumentException("Unable to locate user defined type '" + element.EscapedFullName + "'");
				}
			}

			return collection;
		}



		private UserDefinedTableTypeModel ToModel(Smo.UserDefinedTableType tableType)
		{
			var model = new UserDefinedTableTypeModel();
			model.Name = new DatabaseName(tableType.Schema, tableType.Name);
			model.Columns = ToModel(tableType.Columns);
			return model;
		}

		private IEnumerable<ColumnModel> ToModel(Smo.ColumnCollection columns)
		{
			return columns.Cast<Smo.Column>().Select(c => new ColumnModel()
			{
				DatabaseName = c.Name,
				//TODO
			});
		}



		private readonly Dictionary<string, Smo.UserDefinedTableType> tableTypeLookup;

	}

}
