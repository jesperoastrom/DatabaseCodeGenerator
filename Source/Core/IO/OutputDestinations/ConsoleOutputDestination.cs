namespace SqlFramework.IO.OutputDestinations
{
    public sealed class ConsoleOutputDestination : IOutputDestination
    {
        public void Write(string s)
        {
            System.Console.Write(s);
        }

        public void WriteLine(string s)
        {
            System.Console.WriteLine(s);
        }
    }
}