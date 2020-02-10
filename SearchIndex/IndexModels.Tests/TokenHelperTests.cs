using System.Linq;
using Xunit;

namespace IndexModels.Tests
{
    public class TokenHelperTests
    {
        [Fact]
        public void TestGetTokensFromText()
        {
            var tokens = TokenHelper.GetTokensFromText("a <em>test</em> and another <em>test</em>", "<em>", "</em>").ToList();

            Assert.True(tokens.Count > 0);
        }
    }
}
