﻿using System.Collections.Generic;
using Autofac;
using Flip.Tools.Database.CodeGenerator.Configuration;
using Flip.Tools.Database.CodeGenerator.IO;
using NDesk.Options;
using Flip.Tools.Database.CodeGenerator.Data;


namespace Flip.Tools.Database.CodeGenerator.Console
{

	internal class Program
	{

		public static void Main(string[] args)
		{
			Arguments arguments;
			if (TryParseArguments(args, out arguments))
			{
				if (ValidateArguments(arguments))
				{
					IContainer container = CreateContainer(arguments.ConnectionString);

					var writer = container.Resolve<IDatabaseWriter>();

					if (writer.WriteOutput(arguments.File, arguments.Output, "\t")) //TODO: parameterize indentation
					{
						System.Console.WriteLine("Finished! Press the any-key to continue");
					}
				}
			}
			System.Console.ReadKey();
		}

		private static IContainer CreateContainer(string connectionString)
		{
			var builder = new ContainerBuilder();

			builder.Register(c => new ConnectionStringProvider(connectionString)).As<IConnectionStringProvider>().SingleInstance();

			builder.RegisterCodeGeneratorTypes();
			return builder.Build();
		}



		private static bool TryParseArguments(string[] args, out Arguments arguments)
		{
			Arguments parsedArguments = new Arguments();

			var optionSet = new OptionSet() 
			{
				{ "c|connectionString=", value => parsedArguments.ConnectionString = value },
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
			System.Console.WriteLine("--c|connectionString=<connection string> --f|file=<path to configuration file> --o|output=<path to output file> [--h|help]");
		}

		private static bool ValidateArguments(Arguments arguments)
		{
			bool isValid = true;

			if (arguments.ShowHelp)
			{
				ShowHelp();
				isValid = false;
			}

			if (string.IsNullOrWhiteSpace(arguments.Output))
			{
				System.Console.WriteLine("Missing command line argument 'output'. Type --help for help text.");
				isValid = false;
			}

			if (string.IsNullOrWhiteSpace(arguments.ConnectionString))
			{
				System.Console.WriteLine("Missing command line argument 'connectionString'. Type --help for help text.");
				isValid = false;
			}

			if (string.IsNullOrWhiteSpace(arguments.Output))
			{
				System.Console.WriteLine("Missing command line argument 'output'. Type --help for help text.");
				isValid = false;
			}

			return isValid;
		}



		private sealed class Arguments
		{

			public string ConnectionString { get; set; }
			public string File { get; set; }
			public string Output { get; set; }
			public bool ShowHelp { get; set; }
			public List<string> Extra { get; set; }

		}

	}

}
