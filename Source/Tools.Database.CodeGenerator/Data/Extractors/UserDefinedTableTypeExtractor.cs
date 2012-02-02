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



		public IEnumerable<UserDefinedTableTypeModel> Extract(IEnumerable<Configuration.UserDefinedTypeElement> elements)
		{
			var list = new List<UserDefinedTableTypeModel>(elements.Count());

			foreach (var element in elements)
			{
				if (this.tableTypeLookup.ContainsKey(element.EscapedFullName))
				{
					Smo.UserDefinedTableType tableType = this.tableTypeLookup[element.EscapedFullName];
					list.Add(ToModel(tableType));
				}
				else
				{
					throw new ArgumentException("Unable to locate user defined type '" + element.EscapedFullName + "'");
				}
			}

			return list;
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
				Name = c.Name
			});
		}



		private readonly Dictionary<string, Smo.UserDefinedTableType> tableTypeLookup;

	}

}
