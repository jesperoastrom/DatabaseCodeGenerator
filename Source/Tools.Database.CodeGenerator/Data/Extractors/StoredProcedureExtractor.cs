using System;
using System.Collections.Generic;
using System.Linq;
using Flip.Tools.Database.CodeGenerator.Data.Models;
using Smo = Microsoft.SqlServer.Management.Smo;



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



		public IEnumerable<StoredProcedureModel> Extract(IEnumerable<Configuration.StoredProcedureElement> elements)
		{
			var list = new List<StoredProcedureModel>(elements.Count());

			foreach (var element in elements)
			{
				if (this.storedProcedureLookup.ContainsKey(element.EscapedFullName))
				{
					Smo.StoredProcedure procedure = this.storedProcedureLookup[element.EscapedFullName];
					list.Add(ToModel(procedure));
				}
				else
				{
					throw new ArgumentException("Unable to locate stored procedure '" + element.EscapedFullName + "'");
				}
			}

			return list;
		}



		private StoredProcedureModel ToModel(Smo.StoredProcedure procedure)
		{
			var model = new StoredProcedureModel();
			model.Name = new DatabaseName(procedure.Schema, procedure.Name);
			model.Parameters = ToModel(procedure.Parameters);
			return model;
		}

		private IEnumerable<ParameterModel> ToModel(Smo.StoredProcedureParameterCollection parameters)
		{
			return parameters.Cast<Smo.StoredProcedureParameter>().Select(c => new ParameterModel()
			{
				
			});
		}



		private readonly Dictionary<string, Smo.StoredProcedure> storedProcedureLookup;

	}

}
