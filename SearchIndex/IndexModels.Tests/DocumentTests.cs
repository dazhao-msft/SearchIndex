using Xunit;

namespace IndexModels.Tests
{
    public class DocumentTests
    {
        [Fact]
        public void TestReadMetadataAsDocuments()
        {
            var documents = Document.ReadMetadataAsDocuments();

            Assert.True(documents.Count > 0);
        }
    }
}
