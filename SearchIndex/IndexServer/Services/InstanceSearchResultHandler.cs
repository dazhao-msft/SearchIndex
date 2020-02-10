using IndexModels;
using IndexServer.Models;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Document = IndexModels.Document;

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
                            foreach ((string matchedText, int startOffset) in FindMatchedTexts(context, fragment))
                            {
                                var matchedTerm = new MatchedTerm
                                {
                                    Text = matchedText,
                                    StartIndex = startOffset,
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
                                    Value = fieldValue,
                                    IsExactlyMatch = StringComparer.OrdinalIgnoreCase.Equals(matchedText, fieldValue),
                                    IsSynonymMatch = false,
                                });

                                context.MatchedTerms.Add(matchedTerm);
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<(string, int)> FindMatchedTexts(SearchResultHandlerContext context, string fragment)
        {
            var fragmentTokens = TokenHelper.GetTokensFromText(fragment, context.SearchParameters.HighlightPreTag, context.SearchParameters.HighlightPostTag);

            var subsetOfSearchTokens = new HashSet<TokenInfo>();

            foreach (var fragmentToken in fragmentTokens)
            {
                foreach (var searchToken in context.SearchTokens)
                {
                    if (StringComparer.OrdinalIgnoreCase.Equals(searchToken.Token, fragmentToken.Token))
                    {
                        subsetOfSearchTokens.Add(searchToken);
                    }
                }
            }

            var sortedSubsetOfSearchTokens = subsetOfSearchTokens.OrderBy(p => p.StartOffset).ToList();

            for (int i = 1; i < sortedSubsetOfSearchTokens.Count; i++)
            {
                if (sortedSubsetOfSearchTokens[i - 1].EndOffset > sortedSubsetOfSearchTokens[i].StartOffset)
                {
                    _logger.LogWarning("Assumption broken");
                }
            }

            for (int i = 0; i < sortedSubsetOfSearchTokens.Count; i++)
            {
                for (int j = i; j < sortedSubsetOfSearchTokens.Count; j++)
                {
                    int startOffset = (int)sortedSubsetOfSearchTokens[i].StartOffset;
                    int endOffset = (int)sortedSubsetOfSearchTokens[j].EndOffset;
                    string matchedText = context.SearchText[startOffset..endOffset];

                    yield return (matchedText, startOffset);
                }
            }
        }
    }
}
