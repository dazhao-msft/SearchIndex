using IndexModels;
using IndexServer.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IndexServer.Services
{
    public class InstanceSearchResultHandler : ISearchResultHandler
    {
        private readonly ILogger<InstanceSearchResultHandler> _logger;

        public InstanceSearchResultHandler(ILogger<InstanceSearchResultHandler> logger)
        {
            _logger = logger;
        }

        public async Task ProcessAsync(SearchResultHandlerContext context)
        {
            await Task.Yield();

            foreach (var searchResult in context.SearchResults)
            {
                string entityName = searchResult.Document[Document.EntityNameFieldName].ToString();

                if (entityName == Document.MetadataEntityName)
                {
                    continue;
                }

                string cdsEntityName = entityName;

                foreach (var highlight in searchResult.Highlights)
                {
                    if (Document.TryResolveCdsAttributeName(highlight.Key, cdsEntityName, out string cdsAttributeName))
                    {
                        string fieldValue = searchResult.Document[highlight.Key].ToString();

                        foreach (string fragment in highlight.Value)
                        {
                            foreach (string tokenValue in GetTokenValuesFromFragment(fragment, context.SearchParameters.HighlightPreTag, context.SearchParameters.HighlightPostTag))
                            {
                                //
                                // Question: what if the same word shows in multiple positions?
                                //
                                var token = context.Tokens.FirstOrDefault(p => p.Token == tokenValue);

                                if (token == null)
                                {
                                    _logger.LogWarning($"Token value '{tokenValue}' isn't matched.");
                                    continue;
                                }

                                //
                                // Question: why offset is nullable?
                                //
                                string matchedText = context.SearchText[(int)token.StartOffset..(int)token.EndOffset];

                                var matchedTerm = new MatchedTerm
                                {
                                    Text = matchedText,
                                    StartIndex = (int)token.StartOffset,
                                    Length = matchedText.Length,
                                    TermBindings = new HashSet<TermBinding>(),
                                };

                                matchedTerm.TermBindings.Add(new TermBinding()
                                {
                                    BindingType = BindingType.InstanceValue,
                                    SearchScope = new SearchScope()
                                    {
                                        Table = cdsEntityName,
                                        Column = cdsAttributeName,
                                    },
                                    Value = token.Token,
                                    IsExactlyMatch = token.Token == matchedText,
                                    IsSynonymMatch = false,
                                });

                                context.MatchedTerms.Add(matchedTerm);
                            }
                        }
                    }
                }
            }
        }

        private static IEnumerable<string> GetTokenValuesFromFragment(string fragment, string preTag, string postTag)
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
