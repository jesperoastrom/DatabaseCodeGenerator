using System.Globalization;
using System.Windows.Controls;



namespace Tools.Database.CodeGenerator.Gui.Rules
{

	public sealed class RequiredRule : ValidationRule
	{

		public string ErrorMessage { get; set; }

		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			string s = value as string;
			if (string.IsNullOrWhiteSpace(s))
			{
				return new ValidationResult(false, this.ErrorMessage);
			}
			else
			{
				return new ValidationResult(true, null);
			}
		}

	}

}
