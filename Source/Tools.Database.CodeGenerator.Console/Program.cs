using System.Collections.Generic;
using Flip.Tools.Database.CodeGenerator.Configuration;
using Flip.Tools.Database.CodeGenerator.IO;
using NDesk.Options;



namespace Flip.Tools.Database.CodeGenerator.Console
{

	internal class Program
	{

		public static void Main(string[] args)
		{
			Arguments arguments;
			if (TryParseArguments(args, out arguments))
			{
				if (arguments.ShowHelp)
				{
					ShowHelp();
				}
				else if (string.IsNullOrWhiteSpace(arguments.File))
				{
					System.Console.WriteLine("Missing command line argument 'file'. Type --help for help text.");
				}
				else if (string.IsNullOrWhiteSpace(arguments.Output))
				{
					System.Console.WriteLine("Missing command line argument 'output'. Type --help for help text.");
				}
				else
				{
					var configurationReader = new ConfigurationReader(arguments.File, System.Console.Out);

					DatabaseConfiguration configuration;
					if (configurationReader.TryRead(out configuration))
					{
						var writer = new DatabaseWriter(configuration);

						writer.WriteFile(arguments.Output, "\t");//TODO: parameterize indentation
					}
				}
				System.Console.WriteLine("Finished! Press the any-key to continue");
				System.Console.ReadKey();
			}
		}



		private static bool TryParseArguments(string[] args, out Arguments arguments)
		{
			Arguments parsedArguments = new Arguments();

			var optionSet = new OptionSet() 
			{
				{ "f|file=", value => parsedArguments.File = value },
				{ "o|output=", value => parsedArguments.Output = value },
				{ "h|?|help", value => parsedArguments.ShowHelp = value != null },
			};

			try
			{
				parsedArguments.Extra = optionSet.Parse(args);
				arguments = parsedArguments;
				return true;
			}
			catch (OptionException e)
			{
				System.Console.WriteLine(e.Message);
				System.Console.WriteLine("Try --help' for more information.");
				arguments = null;
				return false;
			}
		}

		private static void ShowHelp()
		{
			System.Console.WriteLine("--f|file=<path to configuration file> --o|output=<path to output file> [--h|help]");
		}



		private sealed class Arguments
		{

			public string File { get; set; }
			public string Output { get; set; }
			public bool ShowHelp { get; set; }
			public List<string> Extra { get; set; }

		}

	}

}
