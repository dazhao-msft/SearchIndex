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
                            string[] synonyms = fieldValue.Split(Document.SynonymDelimiter, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();

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

                            string firstSynonymFragment = highlight.Value[0]?.Split(Document.SynonymDelimiter, StringSplitOptions.RemoveEmptyEntries)?[0];

                            if (!string.IsNullOrEmpty(firstSynonymFragment))
                            {
                                foreach ((string matchedText, int startOffset) in FindMatchedTexts(context, firstSynonymFragment))
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
                                        Value = synonyms[0],
                                        IsExactlyMatch = StringComparer.OrdinalIgnoreCase.Equals(matchedText, synonyms[0]),
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
                                }
                            }
                        }
                    }
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

        private IEnumerable<(string, int)> FindMatchedTexts(SearchResultHandlerContext context, string fragment)
        {
            var fragmentTokens = FragmentHelper.GetTokensFromFragment(fragment, context.SearchParameters.HighlightPreTag, context.SearchParameters.HighlightPostTag);

            var fragmentTokenBindings = CreateTokenBindings(fragmentTokens.ToList(), context.SearchTokens);

            for (int length = fragmentTokenBindings.Count; length > 0; length--)
            {
                for (int index = 0; index + length - 1 < fragmentTokenBindings.Count; index++)
                {
                    var startFragmentTokenBindings = fragmentTokenBindings[index];
                    var endFragmentTokenBindings = fragmentTokenBindings[index + length - 1];

                    if (startFragmentTokenBindings == endFragmentTokenBindings)
                    {
                        foreach (var startSearchToken in startFragmentTokenBindings.SearchTokens)
                        {
                            yield return (context.SearchText[((int)startSearchToken.StartOffset)..((int)startSearchToken.EndOffset)], (int)startSearchToken.StartOffset);
                        }
                    }
                    else
                    {
                        string fragmentPadding = fragment[((int)startFragmentTokenBindings.FragmentToken.EndOffset)..((int)endFragmentTokenBindings.FragmentToken.StartOffset)];
                        fragmentPadding = fragmentPadding.Replace(context.SearchParameters.HighlightPreTag, string.Empty).Replace(context.SearchParameters.HighlightPostTag, string.Empty);

                        foreach (var startSearchToken in startFragmentTokenBindings.SearchTokens)
                        {
                            foreach (var endSearchToken in endFragmentTokenBindings.SearchTokens)
                            {
                                if (startSearchToken.EndOffset <= endSearchToken.StartOffset)
                                {
                                    string searchTextPadding = context.SearchText[((int)startSearchToken.EndOffset)..((int)endSearchToken.StartOffset)];

                                    if (ArePaddingsEquivalent(searchTextPadding.AsSpan(), fragmentPadding.AsSpan()))
                                    {
                                        yield return (context.SearchText[((int)startSearchToken.StartOffset)..((int)endSearchToken.EndOffset)], (int)startSearchToken.StartOffset);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static bool ArePaddingsEquivalent(ReadOnlySpan<char> padding1, ReadOnlySpan<char> padding2)
        {
            if (padding1.IsWhiteSpace() && padding2.IsWhiteSpace())
            {
                return true;
            }

            return padding1.Equals(padding2, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Given a sorted list of fragment tokens and a sorted list of search tokens, creates bindings for each of the fragment tokens.
        /// </summary>
        private static IReadOnlyList<FragmentTokenBindings> CreateTokenBindings(IReadOnlyList<TokenInfo> fragmentTokens, IReadOnlyList<TokenInfo> searchTokens)
        {
            var bindings = new List<FragmentTokenBindings>();

            foreach (var fragmentToken in fragmentTokens)
            {
                var subsetOfSearchTokens = searchTokens.Where(searchToken => StringComparer.OrdinalIgnoreCase.Equals(fragmentToken.Token, searchToken.Token)).ToList();

                if (subsetOfSearchTokens.Count > 0)
                {
                    bindings.Add(new FragmentTokenBindings { FragmentToken = fragmentToken, SearchTokens = subsetOfSearchTokens });
                }
            }

            return bindings;
        }

        /// <summary>
        /// Mapping info between one fragment token to one or multiple search tokens. Search tokens are in order.
        /// </summary>
        private class FragmentTokenBindings
        {
            public TokenInfo FragmentToken { get; set; }

            public IReadOnlyList<TokenInfo> SearchTokens { get; set; }
        }
    }
}
