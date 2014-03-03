﻿using System.Collections.Generic;



namespace SqlFramework.Data.Models
{

	public sealed class StoredProcedureModel
	{

		public DatabaseName DatabaseName { get; set; }
		public TypeName TypeName { get; set; }
		public List<ParameterModel> Parameters { get; set; }
		public List<ParameterModel> OutputParameters { get; set; }
		public List<StoredProcedureResultModel> Results { get; set; }



		public override string ToString()
		{
			return this.DatabaseName == null ? null : this.DatabaseName.ToString();
		}

	}

}