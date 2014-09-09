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
            _codeBuilder = new StringBuilder(1024);
            _indentation = indentation;
            _indentLookup = new Dictionary<byte, string>(8);
        }

        public string GetString()
        {
            return _codeBuilder.ToString();
        }

        private string GetIndentation()
        {
            if (_indentLookup.ContainsKey(Indent))
            {
                return _indentLookup[Indent];
            }
            string s = string.Concat(Enumerable.Repeat(_indentation, Indent));
            _indentLookup.Add(Indent, s);
            return s;
        }

        public byte Indent { get; set; }

        public ICodeWriter WriteIndentation()
        {
            _codeBuilder.Append(GetIndentation());

            return this;
        }

        public ICodeWriter Write(string s)
        {
            _codeBuilder.Append(s);

            return this;
        }

        public ICodeWriter WriteNewLine()
        {
            _codeBuilder.Append(Environment.NewLine);

            return this;
        }

        public ICodeWriter WriteIndentedLine(string s)
        {
            WriteIndentation();
            _codeBuilder.Append(s);
            _codeBuilder.Append(Environment.NewLine);

            return this;
        }

        void IDisposable.Dispose()
        {
            GC.SuppressFinalize(obj: this);
        }

        private readonly StringBuilder _codeBuilder;
        private readonly Dictionary<byte, string> _indentLookup;
        private readonly string _indentation;
    }
}