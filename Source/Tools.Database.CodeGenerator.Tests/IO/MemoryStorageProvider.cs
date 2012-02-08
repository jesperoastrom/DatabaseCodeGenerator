using System;
using System.Collections.Generic;
using System.IO;
using Flip.Tools.Database.CodeGenerator.IO;
using System.Reflection;



namespace Flip.Tools.Database.CodeGenerator.Tests.IO
{

	public sealed class MemoryStorageProvider : IStorageProvider, IDisposable
	{

		public MemoryStorageProvider()
		{
			this.codeWriters = new Dictionary<string, StringCodeWriter>(StringComparer.OrdinalIgnoreCase);
		}



		public bool OutputDirectoryExists(string directory)
		{
			return true;
		}

		public bool ConfigurationFileExists(string fileName)
		{
			return true;
		}

		public ICodeWriter CreateOrOpenOutputWriter(string fileName, string indentation)
		{
			if (this.codeWriters.ContainsKey(fileName))
			{
				return this.codeWriters[fileName];
			}
			var codeWriter = new StringCodeWriter(indentation);
			this.codeWriters.Add(fileName, codeWriter);
			return codeWriter;
		}

		public Stream OpenConfigurationFile(string fileName)
		{
			return EmbeddedResourceHelper.GetStreamFromEmbeddedResource(fileName);
		}

		public string GetOutputString(string fileName)
		{
			if (this.codeWriters.ContainsKey(fileName))
			{
				return this.codeWriters[fileName].GetString();
			}
			return null;
		}



		private void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					if (this.codeWriters.Count > 0)
					{
						foreach (IDisposable codeWriter in this.codeWriters.Values)
						{
							try
							{
								codeWriter.Dispose();
							}
							catch { }
						}
					}
				}
				this.codeWriters.Clear();
				this.disposed = true;
			}
		}



		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}



		private bool disposed;
		private readonly Dictionary<string, StringCodeWriter> codeWriters;

	}

}
