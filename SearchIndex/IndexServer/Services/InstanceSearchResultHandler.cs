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

                        if (ContainsSynonyms(cdsEntityName, cdsAttributeName, fieldValue))
                        {
                            string[] synonyms = fieldValue.Split(Document.SynonymDelimiter, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray();

                            for (int i = 0; i < synonyms.Length; i++)
                            {
                                int startOffset = context.SearchText.IndexOf(synonyms[i], StringComparison.OrdinalIgnoreCase);

                                if (startOffset >= 0)
                                {
                                    string matchedText = context.SearchText.Substring(startOffset, synonyms[i].Length);

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
                                        Value = synonyms[0], // The actual value is the first synonym.
                                        IsExactlyMatch = true,
                                        IsSynonymMatch = !StringComparer.OrdinalIgnoreCase.Equals(synonyms[0], synonyms[i]),
                                    });

                                    context.MatchedTerms.Add(matchedTerm);
                                }
                            }

                            //
                            // TODO: Design a better data structure to support synonym in the same field.
                            //

                            if (context.SearchText.IndexOf(synonyms[0], StringComparison.OrdinalIgnoreCase) < 0)
                            {
                                string firstSynonymFragment = highlight.Value[0]?.Split(Document.SynonymDelimiter, StringSplitOptions.RemoveEmptyEntries)?[0];

                                if (!string.IsNullOrEmpty(firstSynonymFragment) &&
                                    firstSynonymFragment.Contains(context.SearchParameters.HighlightPreTag, StringComparison.Ordinal) &&
                                    firstSynonymFragment.Contains(context.SearchParameters.HighlightPostTag, StringComparison.Ordinal))
                                {
                                    int startOffset = firstSynonymFragment.IndexOf(context.SearchParameters.HighlightPreTag, StringComparison.Ordinal);
                                    startOffset += context.SearchParameters.HighlightPreTag.Length;

                                    int endOffset = firstSynonymFragment.IndexOf(context.SearchParameters.HighlightPostTag, StringComparison.Ordinal);

                                    string matchedText = firstSynonymFragment[startOffset..endOffset];

                                    var matchedTerm = new MatchedTerm
                                    {
                                        Text = matchedText,
                                        StartIndex = context.SearchText.IndexOf(matchedText, StringComparison.OrdinalIgnoreCase),
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
                                        Value = synonyms[0],
                                        IsExactlyMatch = false,
                                        IsSynonymMatch = false,
                                    });

                                    context.MatchedTerms.Add(matchedTerm);
                                }
                            }
                        }
                        else
                        {
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

                                    //
                                    // The longest matched text is returned first. If the given matched text is a sub string of field value, it is
                                    // unnecessary to continue.
                                    //

                                    if (fieldValue.IndexOf(matchedText, StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<(string, int)> FindMatchedTexts(SearchResultHandlerContext context, string fragment)
        {
            var fragmentTokens = FragmentHelper.GetTokensFromFragment(fragment, context.SearchParameters.HighlightPreTag, context.SearchParameters.HighlightPostTag);

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

            for (int i = sortedSubsetOfSearchTokens.Count; i > 0; i--)
            {
                for (int j = 0; i + j - 1 < sortedSubsetOfSearchTokens.Count; j++)
                {
                    int startOffset = (int)sortedSubsetOfSearchTokens[j].StartOffset;
                    int endOffset = (int)sortedSubsetOfSearchTokens[i + j - 1].EndOffset;
                    string matchedText = context.SearchText[startOffset..endOffset];

                    yield return (matchedText, startOffset);
                }
            }
        }

        private bool ContainsSynonyms(string cdsEntityName, string cdsAttributeName, string fieldValue)
        {
            if (cdsAttributeName == "address1_city" ||
                cdsAttributeName == "address1_stateorprovince" ||
                cdsAttributeName == "address1_country" ||
                (cdsEntityName == "account" && cdsAttributeName == "name"))
            {
                return fieldValue.Split(Document.SynonymDelimiter, StringSplitOptions.RemoveEmptyEntries).Length > 1;
            }

            return false;
        }
    }
}
