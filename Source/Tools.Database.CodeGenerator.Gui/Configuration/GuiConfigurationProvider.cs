using System.Xml.Serialization;
using Flip.Tools.Database.CodeGenerator.IO;



namespace Tools.Database.CodeGenerator.Gui.Configuration
{

	public sealed class GuiConfigurationProvider
	{

		public GuiConfigurationProvider(IStorageProvider storageProvider)
		{
			this.storageProvider = storageProvider;
		}


		public GuiConfiguration LoadConfiguration()
		{
			GuiConfiguration configuration;
			if (!this.TryReadConfiguration(GetFilenameInCurrentExecutableFolder(GuiConfigurationProvider.configurationFilename), out configuration))
			{
				return new GuiConfiguration()
				{
					ConnectionString = "Data Source=localhost;Initial Catalog=<DatabaseName>;Integrated Security=SSPI;",
				};
			}
			return configuration;
		}

		public void SaveConfiguration(GuiConfiguration configuration)
		{
			var serializer = new XmlSerializer(typeof(GuiConfiguration));

			using (var stream = this.storageProvider.CreateOrOpenStream(GuiConfigurationProvider.configurationFilename))
			{
				serializer.Serialize(stream, configuration);
			}
		}



		private bool TryReadConfiguration(string file, out GuiConfiguration configuration)
		{
			if (!this.storageProvider.FileExists(file))
			{
				configuration = null;
				return false;
			}

			var serializer = new XmlSerializer(typeof(GuiConfiguration));

			try
			{
				using (var stream = this.storageProvider.OpenStream(file))
				{
					configuration = (GuiConfiguration)serializer.Deserialize(stream);
					return true;
				}
			}
			catch
			{
				configuration = null;
				return false;
			}
		}

		private string GetFilenameInCurrentExecutableFolder(string filename)
		{
			return this.storageProvider.Combine(
				this.storageProvider.GetDirectoryName(typeof(GuiConfigurationProvider).Assembly.Location),
				filename);
		}



		private const string configurationFilename = "gui-configuration.xml";
		private readonly IStorageProvider storageProvider;

	}

}
