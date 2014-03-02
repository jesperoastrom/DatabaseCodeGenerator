using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace SqlFramework.IO
{

	public sealed class StringCodeWriter : ICodeWriter
	{

		public StringCodeWriter(string indentation)
		{
			this.codeBuilder = new StringBuilder(1024);
			this.indentation = indentation;
			this.indentLookup = new Dictionary<byte, string>(8);
		}



		public byte Indent { get; set; }

		public ICodeWriter WriteIndentation()
		{
			this.codeBuilder.Append(GetIndentation());

			return this;
		}

		public ICodeWriter Write(string s)
		{
			this.codeBuilder.Append(s);

			return this;
		}

		public ICodeWriter WriteNewLine()
		{
			this.codeBuilder.Append(System.Environment.NewLine);

			return this;
		}

		public ICodeWriter WriteIndentedLine(string s)
		{
			this.WriteIndentation();
			this.codeBuilder.Append(s);
			this.codeBuilder.Append(Environment.NewLine);

			return this;
		}

		public string GetString()
		{
			return codeBuilder.ToString();
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



		void IDisposable.Dispose()
		{
			GC.SuppressFinalize(this);
		}



		private StringBuilder codeBuilder;
		private readonly Dictionary<byte, string> indentLookup;
		private readonly string indentation;

	}

}
