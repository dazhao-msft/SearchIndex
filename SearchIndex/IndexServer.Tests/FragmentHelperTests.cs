using System.Linq;
using Xunit;

namespace IndexServer.Services.Tests
{
    public class FragmentHelperTests
    {
        [Fact]
        public void TestGetTokensFromText()
        {
            var tokens = FragmentHelper.GetTokensFromFragment("a <em>test</em> and another <em>test</em>", "<em>", "</em>").ToList();

            Assert.True(tokens.Count > 0);
        }
    }
}
