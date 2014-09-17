namespace SqlFramework.Gui.Configuration
{
    using System.Xml.Serialization;
    using SqlFramework.IO.StorageProviders;

    public sealed class GuiConfigurationProvider
    {
        public GuiConfigurationProvider(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        public GuiConfiguration LoadConfiguration()
        {
            GuiConfiguration configuration;
            if (!TryReadConfiguration(GetFilenameInCurrentExecutableFolder(ConfigurationFilename), out configuration))
            {
                return new GuiConfiguration
                           {
                               ConnectionString = "Data Source=localhost;Initial Catalog=<DatabaseName>;Integrated Security=SSPI;",
                           };
            }
            return configuration;
        }

        public void SaveConfiguration(GuiConfiguration configuration)
        {
            var serializer = new XmlSerializer(typeof(GuiConfiguration));

            using (var stream = _storageProvider.CreateOrOpenStream(ConfigurationFilename))
            {
                serializer.Serialize(stream, configuration);
            }
        }

        private string GetFilenameInCurrentExecutableFolder(string filename)
        {
            return _storageProvider.Combine(
                _storageProvider.GetDirectoryName(typeof(GuiConfigurationProvider).Assembly.Location),
                filename);
        }

        private bool TryReadConfiguration(string file, out GuiConfiguration configuration)
        {
            if (!_storageProvider.FileExists(file))
            {
                configuration = null;
                return false;
            }

            var serializer = new XmlSerializer(typeof(GuiConfiguration));

            try
            {
                using (var stream = _storageProvider.OpenStream(file))
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

        private const string ConfigurationFilename = "gui-configuration.xml";
        private readonly IStorageProvider _storageProvider;
    }
}