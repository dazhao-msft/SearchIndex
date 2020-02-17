using System.Linq;
using Xunit;

namespace IndexServer.Services.Tests
{
    public class FragmentHelperTests
    {
        [Theory]
        [InlineData("", "<em>", "</em>", new string[] { })]
        [InlineData("a <em>test</em> and another <em>test</em>", "<em>", "</em>", new[] { "test", "test" })]
        public void TestGetTokensFromFragment(string fragment, string preTag, string postTag, string[] expected)
        {
            var tokens = FragmentHelper.GetTokensFromFragment(fragment, preTag, postTag).ToList();

            Assert.True(tokens.Count == expected.Length);

            for (int i = 0; i < tokens.Count; i++)
            {
                Assert.True(tokens[i].Token == expected[i]);
            }
        }
    }
}
