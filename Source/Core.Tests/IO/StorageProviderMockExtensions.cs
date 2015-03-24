namespace SqlFramework.IO
{
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using Moq;
    using StorageProviders;

    internal static class StorageProviderMockExtensions
    {
        public static void SimulateFileExists(this Mock<IStorageProvider> mock, string fileName, bool returnValue)
        {
            mock.Setup(x => x.FileExists(fileName)).Returns(returnValue);
        }

        public static void SimulateOpenStream(this Mock<IStorageProvider> mock, string fileName, string s)
        {
            Debug.Assert(s != null, string.Format("Resource {0} is null", s));
            mock.Setup(x => x.OpenStream(fileName)).Returns(new MemoryStream(Encoding.UTF8.GetBytes(s)));
        }
    }
}