namespace SqlFramework.Configuration
{

	public interface IConfigurationReader
	{

		bool TryRead(string file, out DatabaseConfiguration configuration);

	}

}
