namespace SqlFramework.IO.CodeBuilders
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class StreamCodeBuilder : ICodeBuilder
    {
        public StreamCodeBuilder(Stream stream, string indentation)
        {
            _indentation = indentation;
            _indentLookup = new Dictionary<byte, string>(8);
            _stream = stream;
            _writer = new StreamWriter(stream, Encoding.UTF8);
        }

        private void AssertNotDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("Object has been disposed");
            }
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_writer != null)
                    {
                        try
                        {
                            _writer.Flush();
                            _writer.Dispose();
                        }
                        catch
                        {
                        }
                    }
                    if (_stream != null)
                    {
                        try
                        {
                            _stream.Close();
                        }
                        catch
                        {
                        }
                    }
                }
                _writer = null;
                _stream = null;
                _disposed = true;
            }
        }

        private string GetIndentation()
        {
            if (_indentLookup.ContainsKey(Indent))
            {
                return _indentLookup[Indent];
            }
            else
            {
                string s = string.Concat(Enumerable.Repeat(_indentation, Indent));
                _indentLookup.Add(Indent, s);
                return s;
            }
        }

        public byte Indent { get; set; }

        public ICodeBuilder WriteIndentation()
        {
            AssertNotDisposed();

            _writer.Write(GetIndentation());

            return this;
        }

        public ICodeBuilder Write(string s)
        {
            AssertNotDisposed();

            _writer.Write(s);

            return this;
        }

        public ICodeBuilder WriteNewLine()
        {
            AssertNotDisposed();

            Write(System.Environment.NewLine);

            return this;
        }

        public ICodeBuilder WriteIndentedLine(string s)
        {
            AssertNotDisposed();

            WriteIndentation();
            _writer.Write(s);
            _writer.Write(Environment.NewLine);

            return this;
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private readonly Dictionary<byte, string> _indentLookup;
        private readonly string _indentation;
        private bool _disposed;
        private Stream _stream;
        private StreamWriter _writer;
    }
}