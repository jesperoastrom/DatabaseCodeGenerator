using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;



namespace SqlFramework.IO
{

	public class StreamCodeWriter : ICodeWriter
	{

		public StreamCodeWriter(Stream stream, string indentation)
		{
			this.indentation = indentation;
			this.indentLookup = new Dictionary<byte, string>(8);
			this.stream = stream;
			this.writer = new StreamWriter(stream, Encoding.UTF8);
		}



		public byte Indent { get; set; }

		public ICodeWriter WriteIndentation()
		{
			AssertNotDisposed();

			this.writer.Write(GetIndentation());

			return this;
		}

		public ICodeWriter Write(string s)
		{
			AssertNotDisposed();

			this.writer.Write(s);

			return this;
		}

		public ICodeWriter WriteNewLine()
		{
			AssertNotDisposed();

			this.Write(System.Environment.NewLine);

			return this;
		}

		public ICodeWriter WriteIndentedLine(string s)
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
						try
						{
							this.writer.Flush();
							this.writer.Dispose();
						}
						catch { }
					}
					if (this.stream != null)
					{
						try
						{
							this.stream.Close();
						}
						catch { }
					}
				}
				this.writer = null;
				this.stream = null;
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
		private Stream stream;
		private readonly Dictionary<byte, string> indentLookup;
		private readonly string indentation;

	}

}
