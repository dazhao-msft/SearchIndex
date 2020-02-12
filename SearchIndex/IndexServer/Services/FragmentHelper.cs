using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;

namespace IndexServer.Services
{
    public static class FragmentHelper
    {
        /// <summary>
        /// Extracts the tokens that are enclosed with <paramref name="preTag"/> and <paramref name="postTag"/>.
        /// </summary>
        public static IEnumerable<TokenInfo> GetTokensFromFragment(string fragment, string preTag, string postTag)
        {
            int position = 0;
            int startOffset = 0;

            var fragmentAsMemory = fragment.AsMemory();

            while (true)
            {
                int preTagIndex = fragmentAsMemory.Span.IndexOf(preTag);
                if (preTagIndex < 0)
                {
                    yield break;
                }

                startOffset += preTagIndex + preTag.Length;
                fragmentAsMemory = fragmentAsMemory.Slice(preTagIndex + preTag.Length);

                int postTagIndex = fragmentAsMemory.Span.IndexOf(postTag);
                if (postTagIndex < 0)
                {
                    throw new InvalidOperationException("pre tag and post tag don't match.");
                }

                yield return new TokenInfo(fragmentAsMemory.Slice(0, postTagIndex).ToString(), startOffset, startOffset + postTagIndex, position);

                startOffset += postTagIndex + postTag.Length;
                fragmentAsMemory = fragmentAsMemory.Slice(postTagIndex + postTag.Length);

                position++;
            }
        }
    }
}
