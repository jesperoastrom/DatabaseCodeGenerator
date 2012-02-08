using System;



namespace Flip.Tools.Database.CodeGenerator.IO
{

	public interface ICodeWriter : IDisposable
	{

		byte Indent { get; set; }
		ICodeWriter WriteIndentation();
		ICodeWriter Write(string s);
		ICodeWriter WriteNewLine();
		ICodeWriter WriteIndentedLine(string s);

	}

}
