using System;
using System.Collections.Generic;
using System.Linq;
using Flip.Tools.Database.CodeGenerator.Data.Models;
using Smo = Microsoft.SqlServer.Management.Smo;
using System.Data;



namespace Flip.Tools.Database.CodeGenerator.Data.Extractors
{

	internal sealed class StoredProcedureExtractor
	{

		public StoredProcedureExtractor(Smo.StoredProcedureCollection procedures)
		{
			this.storedProcedureLookup = procedures
				.Cast<Smo.StoredProcedure>()
				.ToDictionary(t => t.Schema.EscapeDatabaseName() + "." + t.Name.EscapeDatabaseName(), StringComparer.OrdinalIgnoreCase);
		}



		public SchemaCollection<StoredProcedureModel> Extract(Configuration.StoredProcedures storedProcedures)
		{
			var collection = new SchemaCollection<StoredProcedureModel>(storedProcedures.Namespace);

			foreach (var element in storedProcedures.Elements)
			{
				if (this.storedProcedureLookup.ContainsKey(element.EscapedFullName))
				{
					Smo.StoredProcedure procedure = this.storedProcedureLookup[element.EscapedFullName];
					collection.AddElement(procedure.Schema, ToModel(storedProcedures.Namespace, procedure));
				}
				else
				{
					throw new ArgumentException("Unable to locate stored procedure '" + element.EscapedFullName + "'");
				}
			}

			return collection;
		}



		private StoredProcedureModel ToModel(string procedureNamespace, Smo.StoredProcedure procedure)
		{
			var model = new StoredProcedureModel();
			model.DatabaseName = new DatabaseName(procedure.Schema, procedure.Name);
			model.TypeName = new TypeName(procedureNamespace, procedure.Name);
			model.Parameters = ToModel(procedure.Parameters);
			return model;
		}

		private List<ParameterModel> ToModel(Smo.StoredProcedureParameterCollection parameters)
		{
			return parameters.Cast<Smo.StoredProcedureParameter>().Select(p => new ParameterModel()
			{
				Column = new ColumnModel()
				{
					DatabaseName = p.Name,
					SqlDbType = ToDbType(p.DataType.SqlDataType),
					ParameterName = p.Name.ToParameterName(),
					PropertyName = p.Name.ToPropertyName(),
					ClrType = p.DataType.ToClrString()
				}
			}).ToList();
		}

		private SqlDbType ToDbType(Smo.SqlDataType sqlDataType)
		{
			SqlDbType sqlDbType;
			switch (sqlDataType)
			{
				case Smo.SqlDataType.UserDefinedType:
					sqlDbType = System.Data.SqlDbType.Udt;
					break;
				//TODO
				//case Smo.SqlDataType.None:
				//case Smo.SqlDataType.NVarCharMax:
				//case Smo.SqlDataType.UserDefinedDataType:
				//case Smo.SqlDataType.VarBinaryMax:
				//case Smo.SqlDataType.VarCharMax:
				//case Smo.SqlDataType.SysName:
				//case Smo.SqlDataType.Numeric:
				//case Smo.SqlDataType.UserDefinedTableType:
				//case Smo.SqlDataType.HierarchyId:
				//case Smo.SqlDataType.Geometry:
				case Smo.SqlDataType.Geography:
					throw new NotSupportedException("Unable to convert to SqlDbType:" + sqlDataType);
				default:
					if (!Enum.TryParse<SqlDbType>(sqlDataType.ToString(), out sqlDbType))
					{
						throw new NotSupportedException("Unable to convert to SqlDbType:" + sqlDataType);
					}
					break;
			}
			return sqlDbType;
		}



		private readonly Dictionary<string, Smo.StoredProcedure> storedProcedureLookup;

	}

}
