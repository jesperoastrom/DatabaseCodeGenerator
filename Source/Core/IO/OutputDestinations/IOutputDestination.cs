namespace SqlFramework.IO.OutputDestinations
{
    public interface IOutputDestination
    {
        void Write(string s);
        
        void WriteLine(string s);
    }
}