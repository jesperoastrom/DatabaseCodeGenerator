using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;



namespace Flip.Tools.Database.CodeGenerator.IO
{

	internal class Writer
	{

		public Writer(Stream stream, string indentation)
		{
			this.indentation = indentation;
			this.indentLookup = new Dictionary<byte, string>(8);
			this.writer = new StreamWriter(stream, Encoding.UTF8);
		}



		public byte Indent { get; set; }

		public void WriteIndentation()
		{
			this.writer.Write(GetIndentation());
		}

		public void Write(string s)
		{
			this.writer.Write(s);
		}

		public void WriteLine(string s)
		{
			this.WriteIndentation();
			this.writer.Write(s);
			this.writer.Write(Environment.NewLine);
		}



		private string GetIndentation()
		{
			if (this.indentLookup.ContainsKey(this.Indent))
			{
				return this.indentLookup[this.Indent];
			}
			else
			{
				string s = string.Concat(Enumerable.Repeat(this.indentation, this.Indent));
				this.indentLookup.Add(this.Indent, s);
				return s;
			}
		}



		private readonly Dictionary<byte, string> indentLookup;
		private readonly string indentation;
		private readonly StreamWriter writer;

	}

}
