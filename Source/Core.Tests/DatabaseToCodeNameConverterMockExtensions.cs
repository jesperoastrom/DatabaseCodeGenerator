namespace SqlFramework
{
    using Moq;

    public static class DatabaseToCodeNameConverterMockExtensions
    {
        public static void SimulateGetShortestNamespaceTo(this Mock<IDatabaseToCodeNameConverter> converter, string fromNs, string toNs, string ns)
        {
            converter.Setup(x => x.GetShortestNamespaceTo(fromNs, toNs)).Returns(ns);
        }
    }
}
