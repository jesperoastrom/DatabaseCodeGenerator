using System.Windows.Controls;
using SqlFramework.IO;



namespace Tools.Database.CodeGenerator.Gui.IO
{

	public sealed class TextBoxTextWriter : ITextWriter
	{

		public TextBoxTextWriter(TextBox control)
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
