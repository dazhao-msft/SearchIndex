using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;

namespace IndexModels
{
    public static class TokenHelper
    {
        /// <summary>
        /// Extracts the tokens that are enclosed with <paramref name="preTag"/> and <paramref name="postTag"/>.
        /// </summary>
        public static IEnumerable<TokenInfo> GetTokensFromText(string text, string preTag, string postTag)
        {
            int position = 0;
            int startOffset = 0;

            var textAsMemory = text.AsMemory();

            while (true)
            {
                int preTagIndex = textAsMemory.Span.IndexOf(preTag);
                if (preTagIndex < 0)
                {
                    yield break;
                }

                startOffset += preTagIndex + preTag.Length;
                textAsMemory = textAsMemory.Slice(preTagIndex + preTag.Length);

                int postTagIndex = textAsMemory.Span.IndexOf(postTag);
                if (postTagIndex < 0)
                {
                    throw new InvalidOperationException("pre tag and post tag don't match.");
                }

                yield return new TokenInfo(textAsMemory.Slice(0, postTagIndex).ToString(), startOffset, startOffset + postTagIndex, position);

                startOffset += postTagIndex + postTag.Length;
                textAsMemory = textAsMemory.Slice(postTagIndex + postTag.Length);

                position++;
            }
        }
    }
}
