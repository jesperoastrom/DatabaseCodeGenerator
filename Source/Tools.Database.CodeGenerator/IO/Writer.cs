using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;



namespace Flip.Tools.Database.CodeGenerator.IO
{

	internal class Writer : IDisposable
	{

		public Writer(Stream stream, string indentation)
		{
			this.indentation = indentation;
			this.indentLookup = new Dictionary<byte, string>(8);
			this.writer = new StreamWriter(stream, Encoding.UTF8);
		}



		public byte Indent { get; set; }

		public Writer WriteIndentation()
		{
			AssertNotDisposed();

			this.writer.Write(GetIndentation());

			return this;
		}

		public Writer Write(string s)
		{
			AssertNotDisposed();

			this.writer.Write(s);

			return this;
		}

		public Writer WriteNewLine()
		{
			AssertNotDisposed();

			this.Write(System.Environment.NewLine);

			return this;
		}

		public Writer WriteIndentedLine(string s)
		{
			AssertNotDisposed();

			this.WriteIndentation();
			this.writer.Write(s);
			this.writer.Write(Environment.NewLine);

			return this;
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

		private void AssertNotDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("Object has been disposed");
			}
		}

		private void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					if (this.writer != null)
					{
						this.writer.Flush();
						this.writer.Dispose();
					}
				}
				this.writer = null;
				this.disposed = true;
			}
		}



		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}



		private bool disposed;
		private StreamWriter writer;
		private readonly Dictionary<byte, string> indentLookup;
		private readonly string indentation;

	}

}
