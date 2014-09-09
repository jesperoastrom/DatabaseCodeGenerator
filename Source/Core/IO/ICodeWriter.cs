using System;

namespace SqlFramework.IO
{
    public interface ICodeWriter : IDisposable
    {
        byte Indent { get; set; }
        ICodeWriter Write(string s);
        ICodeWriter WriteIndentation();
        ICodeWriter WriteIndentedLine(string s);
        ICodeWriter WriteNewLine();
    }
}