using System;
using System.Collections.Generic;
using System.IO;
using SqlFramework.IO;

namespace SqlFramework.IO.Tests
{
    public sealed class MemoryStorageProvider : IStorageProvider, IDisposable
    {
        public MemoryStorageProvider()
        {
            _codeWriters = new Dictionary<string, StringCodeWriter>(StringComparer.OrdinalIgnoreCase);
        }

        public string GetOutputString(string fileName)
        {
            if (_codeWriters.ContainsKey(fileName))
            {
                return _codeWriters[fileName].GetString();
            }
            return null;
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_codeWriters.Count > 0)
                    {
                        foreach (IDisposable codeWriter in _codeWriters.Values)
                        {
                            try
                            {
                                codeWriter.Dispose();
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                _codeWriters.Clear();
                _disposed = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public string Combine(string[] paths)
        {
            return Path.Combine(paths);
        }

        public string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        public bool DirectoryExists(string directory)
        {
            return true;
        }

        public bool FileExists(string fileName)
        {
            return true;
        }

        public ICodeWriter CreateOrOpenCodeWriter(string fileName, string indentation)
        {
            if (_codeWriters.ContainsKey(fileName))
            {
                return _codeWriters[fileName];
            }
            var codeWriter = new StringCodeWriter(indentation);
            _codeWriters.Add(fileName, codeWriter);
            return codeWriter;
        }

        public Stream OpenStream(string fileName)
        {
            return EmbeddedResourceHelper.GetStreamFromEmbeddedResource(fileName);
        }

        public Stream CreateOrOpenStream(string fileName)
        {
            try
            {
                return EmbeddedResourceHelper.GetStreamFromEmbeddedResource(fileName);
            }
            catch (FileNotFoundException)
            {
                throw new NotImplementedException();
            }
        }

        private readonly Dictionary<string, StringCodeWriter> _codeWriters;
        private bool _disposed;
    }
}