using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlFramework.IO.CodeBuilders
{
    public sealed class StringCodeBuilder : ICodeBuilder
    {
        public StringCodeBuilder(string indentation)
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

        public ICodeBuilder WriteIndentation()
        {
            _codeBuilder.Append(GetIndentation());

            return this;
        }

        public ICodeBuilder Write(string s)
        {
            _codeBuilder.Append(s);

            return this;
        }

        public ICodeBuilder WriteNewLine()
        {
            _codeBuilder.Append(Environment.NewLine);

            return this;
        }

        public ICodeBuilder WriteIndentedLine(string s)
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