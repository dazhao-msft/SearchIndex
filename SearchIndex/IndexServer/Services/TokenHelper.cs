using System;
using System.Collections.Generic;

namespace IndexServer.Services
{
    public static class TokenHelper
    {
        public static IEnumerable<string> GetTokenValuesFromFragment(string fragment, string preTag, string postTag)
        {
            var fragmentAsMemory = fragment.AsMemory();

            while (true)
            {
                int preTagIndex = fragmentAsMemory.Span.IndexOf(preTag);
                if (preTagIndex < 0)
                {
                    yield break;
                }

                fragmentAsMemory = fragmentAsMemory.Slice(preTagIndex + preTag.Length);

                int postTagIndex = fragmentAsMemory.Span.IndexOf(postTag);
                if (postTagIndex < 0)
                {
                    throw new InvalidOperationException("pre tag and post tag don't match.");
                }

                yield return fragmentAsMemory.Slice(0, postTagIndex).ToString();

                fragmentAsMemory = fragmentAsMemory.Slice(postTagIndex + postTag.Length);
            }
        }
    }
}
