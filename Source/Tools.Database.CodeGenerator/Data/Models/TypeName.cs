using System;



namespace Flip.Tools.Database.CodeGenerator.Data.Models
{

	public sealed class TypeName : IComparable<TypeName>, IComparable
	{

		public TypeName(string ns, string name)
		{
			this.Namespace = ns;
			this.Name = name.ToTypeName();
			this.FullyQualifiedTypeName = Namespace + "." + Name;
		}



		public string Namespace { get; private set; }

		public string Name { get; private set; }

		public string FullyQualifiedTypeName { get; private set; }




		public override string ToString()
		{
			return this.FullyQualifiedTypeName;
		}



		int IComparable<TypeName>.CompareTo(TypeName other)
		{
			if (other == null)
			{
				return 1;
			}

			return string.Compare(this.FullyQualifiedTypeName, other.FullyQualifiedTypeName);
		}

		int IComparable.CompareTo(object obj)
		{
			return ((IComparable<TypeName>)this).CompareTo(obj as TypeName);
		}



		private string assemblyName;
		private string name;

	}

}
