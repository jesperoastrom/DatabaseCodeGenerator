namespace SqlFramework.Gui.IO
{
    using System.Windows.Controls;
    using SqlFramework.IO.OutputDestinations;

    public sealed class TextBoxOutputDestination : IOutputDestination
    {
        public TextBoxOutputDestination(TextBox control)
        {
            this.control = control;
        }

        public void Write(string s)
        {
            control.AppendText(s);
        }

        public void WriteLine(string s)
        {
            control.AppendText(s);
            control.AppendText(System.Environment.NewLine);
        }

        private readonly TextBox control;
    }
}