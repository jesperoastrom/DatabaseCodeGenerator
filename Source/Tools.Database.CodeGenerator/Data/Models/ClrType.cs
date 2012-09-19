namespace Flip.Tools.Database.CodeGenerator.Data.Models
{

	public sealed class ClrType
	{
		public string TypeName { get; set; }
		public bool IsUserDefined { get; set; }
		public bool IsNullable { get; set; }
		public string InnerTypeName { get; set; }
	}

}
