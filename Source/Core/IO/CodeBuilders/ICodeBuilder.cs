namespace SqlFramework.IO.CodeBuilders
{
    using System;

    public interface ICodeBuilder : IDisposable
    {
        byte Indent { get; set; }
        
        ICodeBuilder Write(string s);
        
        ICodeBuilder WriteIndentation();
        
        ICodeBuilder WriteIndentedLine(string s);
        
        ICodeBuilder WriteNewLine();
    }
}